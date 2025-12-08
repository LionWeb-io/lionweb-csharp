// Copyright 2025 TRUMPF Laser SE and other contributors
// 
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// SPDX-FileCopyrightText: 2024 TRUMPF Laser SE and other contributors
// SPDX-License-Identifier: Apache-2.0

namespace LionWeb.Generator.Impl;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

public static partial class AstExtensions
{
    /// Prepends <paramref name="xdocs"/> before <paramref name="member"/>.
    public static T Xdoc<T>(this T member, IEnumerable<XmlNodeSyntax> xdocs) where T : MemberDeclarationSyntax
    {
        if (member.AttributeLists is { Count: > 0 })
        {
            return member.AppendXdoc(member.AttributeLists, xdocs);
        }

        return member.AppendXdoc(member.Modifiers,
            member.Modifiers.FirstOrDefault(SyntaxFactory.Token(SyntaxKind.None)).Kind(),
            xdocs);
    }

    /// Prepends <paramref name="xdocs"/> before <paramref name="member">member's</paramref> attributes.
    public static T AppendXdoc<T>(this T member, SyntaxList<AttributeListSyntax> attributeLists,
        IEnumerable<XmlNodeSyntax> xdocs) where T : MemberDeclarationSyntax
    {
        var firstAttributeList = attributeLists.First();
        return (T)member
            .WithAttributeLists(SyntaxFactory.List(
                attributeLists.Skip(1)
                    .Prepend(
                        firstAttributeList
                            .WithOpenBracketToken(SyntaxFactory.Token(
                                SyntaxFactory.TriviaList(SyntaxFactory.Trivia(
                                    SyntaxFactory.DocumentationCommentTrivia(
                                        SyntaxKind.SingleLineDocumentationCommentTrivia,
                                        SyntaxFactory.List(
                                            (
                                                firstAttributeList.HasLeadingTrivia
                                                    ? firstAttributeList.GetLeadingTrivia()
                                                        .Select(tr => tr.GetStructure())
                                                        .OfType<DocumentationCommentTriviaSyntax>()
                                                        .SelectMany(t => t.Content)
                                                    : []
                                            ).Concat(xdocs)
                                        )
                                    )
                                )),
                                SyntaxKind.OpenBracketToken,
                                SyntaxFactory.TriviaList()
                            ))
                    )
            ));
    }

    /// Prepends <paramref name="xdocs"/> before <paramref name="member">member's</paramref> modifiers.
    public static T AppendXdoc<T>(this T member, SyntaxTokenList modifiers, SyntaxKind syntaxKind,
        IEnumerable<XmlNodeSyntax> xdocs) where T : MemberDeclarationSyntax
    {
        SyntaxToken firstModifier = modifiers.FirstOrDefault(SyntaxFactory.Token(SyntaxKind.None));
        return (T)member.WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(
                    SyntaxFactory.TriviaList(
                        SyntaxFactory.Trivia(
                            SyntaxFactory.DocumentationCommentTrivia(
                                SyntaxKind.SingleLineDocumentationCommentTrivia,
                                SyntaxFactory.List((firstModifier.HasLeadingTrivia
                                    ? firstModifier.LeadingTrivia
                                        .Select(tr => tr.GetStructure())
                                        .OfType<DocumentationCommentTriviaSyntax>()
                                        .SelectMany(t => t.Content)
                                    : []).Concat(xdocs))
                            )
                        )
                    ),
                    syntaxKind,
                    SyntaxFactory.TriviaList()
                )
            ).AddRange(modifiers.Skip(1))
        );
    }

    /// <returns><c>///&lt;tag&gt;text&lt;/tag&gt;</c></returns>
    public static IEnumerable<XmlNodeSyntax> XdocLine(string text, string? tag = null)
    {
        XmlNodeSyntax body = XdocText(text);
        if (tag != null)
        {
            body = SyntaxFactory.XmlElement(tag, SyntaxFactory.SingletonList(body));
        }

        return
        [
            XdocSlashes(),
            body,
            XdocNewline()
        ];
    }

    /// <returns><c>///&lt;seealso cref="target"/&gt;</c></returns>
    public static IEnumerable<XmlNodeSyntax> XdocSeeAlso(TypeSyntax target) =>
    [
        XdocSlashes(),
        SyntaxFactory.XmlSeeAlsoElement(SyntaxFactory.NameMemberCref(target)),
        XdocNewline()
    ];

    /// <returns><c>///&lt;seealso href="uri"/&gt;</c></returns>
    public static IEnumerable<XmlNodeSyntax> XdocSeeAlso(string uri) =>
    [
        XdocSlashes(),
        SyntaxFactory.XmlEmptyElement("seealso")
            .AddAttributes(SyntaxFactory.XmlTextAttribute("href",
                SyntaxFactory.XmlTextLiteral(SyntaxFactory.TriviaList(), uri, uri, SyntaxFactory.TriviaList()))),
        XdocNewline()
    ];

    /// <returns><c>&lt;inheritdoc/&gt;</c></returns>
    public static IEnumerable<XmlNodeSyntax> XdocInheritDoc() =>
    [
        XdocSlashes(),
        SyntaxFactory.XmlEmptyElement("inheritdoc"),
        XdocNewline()
    ];

    /// <summary>Takes care of properly wrapping multi-line text with `///` prefix</summary>
    /// <returns><c>text</c></returns>
    public static XmlTextSyntax XdocText(string text)
    {
        using var reader = new StringReader(text);
        bool first = true;
        List<SyntaxToken> texts = new();
        while (reader.ReadLine() is { } line)
        {
            if (first)
            {
                texts.Add(SyntaxFactory.XmlTextLiteral(SyntaxFactory.TriviaList(), line, line,
                    SyntaxFactory.TriviaList()));
                first = false;
            } else
            {
                texts.Add(XdocNewlineToken());
                var prefixedLine = $" {line}";
                texts.Add(SyntaxFactory.XmlTextLiteral(SyntaxFactory.TriviaList(XdocSlashesToken()), prefixedLine,
                    prefixedLine, SyntaxFactory.TriviaList()));
            }
        }

        return SyntaxFactory.XmlText()
            .WithTextTokens(
                SyntaxFactory.TokenList(texts)
            );
    }

    /// <returns><c>/// </c></returns>
    public static XmlTextSyntax XdocSlashes() =>
        SyntaxFactory.XmlText()
            .WithTextTokens(
                SyntaxFactory.TokenList(
                    SyntaxFactory.XmlTextLiteral(
                        SyntaxFactory.TriviaList(XdocSlashesToken()), " ", " ", SyntaxFactory.TriviaList()
                    )
                )
            );

    /// <returns><c>///</c></returns>
    private static SyntaxTrivia XdocSlashesToken() =>
        SyntaxFactory.DocumentationCommentExterior("///");

    /// <returns>newline inside Xdoc</returns>
    public static XmlTextSyntax XdocNewline() =>
        SyntaxFactory.XmlText()
            .WithTextTokens(SyntaxFactory.TokenList(XdocNewlineToken()));

    /// <returns>newline Xdoc token</returns>
    private static SyntaxToken XdocNewlineToken() =>
        SyntaxFactory.XmlTextNewLine(SyntaxFactory.TriviaList(), Environment.NewLine, Environment.NewLine,
            SyntaxFactory.TriviaList());
}