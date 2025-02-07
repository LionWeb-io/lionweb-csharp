// Generated by the C# M2TypesGenerator: modify at your own risk!
// ReSharper disable InconsistentNaming
// ReSharper disable SuggestVarOrType_SimpleTypes
// ReSharper disable SuggestVarOrType_Elsewhere
#pragma warning disable 1591
#nullable enable
namespace LionWeb.Core.Test.Languages.Generated.V2024_1.Library.M2;
using LionWeb.Core;
using LionWeb.Core.M2;
using LionWeb.Core.M3;
using LionWeb.Core.Utilities;
using LionWeb.Core.VersionSpecific.V2024_1;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[LionCoreLanguage(Key = "library", Version = "1")]
public partial class LibraryLanguage : LanguageBase<ILibraryFactory>
{
	public static readonly LibraryLanguage Instance = new Lazy<LibraryLanguage>(() => new("txjxNU9yRzEuyghtmgJK_l-nF93qWt7d1vErz5RbLow")).Value;
	public LibraryLanguage(string id) : base(id, LionWebVersions.v2024_1)
	{
		_book = new(() => new ConceptBase<LibraryLanguage>("uEoqgTU-BImmpne9vdcqM-o2JWS8yGAveLounJcncVU", this) { Key = "Book", Name = "Book", Abstract = false, Partition = false, FeaturesLazy = new(() => [Book_author, Book_pages, Book_title, Book_type]) });
		_book_author = new(() => new ReferenceBase<LibraryLanguage>("LGawCEgHtaqYxRhVWTlQSf6NuCpW5MDpi7QWLbGG3DM", Book, this) { Key = "author", Name = "author", Optional = false, Multiple = false, Type = Writer });
		_book_pages = new(() => new PropertyBase<LibraryLanguage>("kGSV8eFs5WIxjUKXyNoBIDW7pL4gyszY2LHKmmzvegU", Book, this) { Key = "pages", Name = "pages", Optional = false, Type = _builtIns.Integer });
		_book_title = new(() => new PropertyBase<LibraryLanguage>("ydTFbSaxS3-iPy4Z0BXupobIaWIpdtL9c_WfbzpBeOw", Book, this) { Key = "title", Name = "title", Optional = false, Type = _builtIns.String });
		_book_type = new(() => new PropertyBase<LibraryLanguage>("cOLUxl-SXt_fm3d3FEBhf6XSdeNKo3N9iW7oJpDfx3k", Book, this) { Key = "type", Name = "type", Optional = true, Type = BookType });
		_bookType = new(() => new EnumerationBase<LibraryLanguage>("6S4CvZSArQGBLCO7FvP8hXw2gMpIzIjXGGz6pyTy-rA", this) { Key = "BookType", Name = "BookType", LiteralsLazy = new(() => [BookType_Normal, BookType_Special]) });
		_bookType_Normal = new(() => new EnumerationLiteralBase<LibraryLanguage>("I5CCX1NbCOzV2c_vtCJKrp0iHdqwYgNPeiQfLL4Gejg", BookType, this) { Key = "Normal", Name = "Normal" });
		_bookType_Special = new(() => new EnumerationLiteralBase<LibraryLanguage>("U5NXStjkSAQJfuMgoi5zUDKJVbckzm_97dtBgm-AVLI", BookType, this) { Key = "Special", Name = "Special" });
		_guideBookWriter = new(() => new ConceptBase<LibraryLanguage>("z4c4dBOvVtSw9UN9T3k5v608cRvRmBgqFIo3nyHo_QA", this) { Key = "GuideBookWriter", Name = "GuideBookWriter", Abstract = false, Partition = false, ExtendsLazy = new(() => Writer), FeaturesLazy = new(() => [GuideBookWriter_countries]) });
		_guideBookWriter_countries = new(() => new PropertyBase<LibraryLanguage>("ajxn2_nvTlZaETaH5NBSdjg-4ecBXAz0PLI9ZISNtfo", GuideBookWriter, this) { Key = "countries", Name = "countries", Optional = false, Type = _builtIns.String });
		_library = new(() => new ConceptBase<LibraryLanguage>("J3-V1k6rqq4Ml8qX_SuUOFhhjXoxLkHrt4mShDRJH1g", this) { Key = "Library", Name = "Library", Abstract = false, Partition = false, FeaturesLazy = new(() => [Library_books, Library_name]) });
		_library_books = new(() => new ContainmentBase<LibraryLanguage>("ZAYPfVsZ-nRe7ICpRe_3LzKufog84Unh9qT4jpT4Izc", Library, this) { Key = "books", Name = "books", Optional = false, Multiple = true, Type = Book });
		_library_name = new(() => new PropertyBase<LibraryLanguage>("Rfy7VBCRh_jJSNcJTaqprdD1YcQS0OnoCMXnbmLCm8Y", Library, this) { Key = "library_Library_name", Name = "name", Optional = false, Type = _builtIns.String });
		_specialistBookWriter = new(() => new ConceptBase<LibraryLanguage>("rs6OGi3yUdrLwpsDHxfQOFx6cb1GsMDT8XzJhcVK988", this) { Key = "SpecialistBookWriter", Name = "SpecialistBookWriter", Abstract = false, Partition = false, ExtendsLazy = new(() => Writer), FeaturesLazy = new(() => [SpecialistBookWriter_subject]) });
		_specialistBookWriter_subject = new(() => new PropertyBase<LibraryLanguage>("JH7BxhpDr9L7f6UBOKtFQBr0TV6r2nAK_6NajM1ij2Q", SpecialistBookWriter, this) { Key = "subject", Name = "subject", Optional = false, Type = _builtIns.String });
		_writer = new(() => new ConceptBase<LibraryLanguage>("7GwNCVubFACsj8FdlnwwIOaPbJ-mLJ5UvU5EnZRjLds", this) { Key = "Writer", Name = "Writer", Abstract = false, Partition = false, FeaturesLazy = new(() => [Writer_name]) });
		_writer_name = new(() => new PropertyBase<LibraryLanguage>("6ephvryVAYXE1bm1LDlUBl0Ib9MOGcM23RDej6kyuqo", Writer, this) { Key = "library_Writer_name", Name = "name", Optional = false, Type = _builtIns.String });
		_factory = new LibraryFactory(this);
	}

	/// <inheritdoc/>
        public override IReadOnlyList<LanguageEntity> Entities => [Book, BookType, GuideBookWriter, Library, SpecialistBookWriter, Writer];
	/// <inheritdoc/>
        public override IReadOnlyList<Language> DependsOn => [];

	private const string _key = "library";
	/// <inheritdoc/>
        public override string Key => _key;

	private const string _name = "library";
	/// <inheritdoc/>
        public override string Name => _name;

	private const string _version = "1";
	/// <inheritdoc/>
        public override string Version => _version;

	private readonly Lazy<Concept> _book;
	public Concept Book => _book.Value;

	private readonly Lazy<Reference> _book_author;
	public Reference Book_author => _book_author.Value;

	private readonly Lazy<Property> _book_pages;
	public Property Book_pages => _book_pages.Value;

	private readonly Lazy<Property> _book_title;
	public Property Book_title => _book_title.Value;

	private readonly Lazy<Property> _book_type;
	public Property Book_type => _book_type.Value;

	private readonly Lazy<Enumeration> _bookType;
	public Enumeration BookType => _bookType.Value;

	private readonly Lazy<EnumerationLiteral> _bookType_Normal;
	public EnumerationLiteral BookType_Normal => _bookType_Normal.Value;

	private readonly Lazy<EnumerationLiteral> _bookType_Special;
	public EnumerationLiteral BookType_Special => _bookType_Special.Value;

	private readonly Lazy<Concept> _guideBookWriter;
	public Concept GuideBookWriter => _guideBookWriter.Value;

	private readonly Lazy<Property> _guideBookWriter_countries;
	public Property GuideBookWriter_countries => _guideBookWriter_countries.Value;

	private readonly Lazy<Concept> _library;
	public Concept Library => _library.Value;

	private readonly Lazy<Containment> _library_books;
	public Containment Library_books => _library_books.Value;

	private readonly Lazy<Property> _library_name;
	public Property Library_name => _library_name.Value;

	private readonly Lazy<Concept> _specialistBookWriter;
	public Concept SpecialistBookWriter => _specialistBookWriter.Value;

	private readonly Lazy<Property> _specialistBookWriter_subject;
	public Property SpecialistBookWriter_subject => _specialistBookWriter_subject.Value;

	private readonly Lazy<Concept> _writer;
	public Concept Writer => _writer.Value;

	private readonly Lazy<Property> _writer_name;
	public Property Writer_name => _writer_name.Value;
}

public partial interface ILibraryFactory : INodeFactory
{
	public Book NewBook(string id);
	public Book CreateBook();
	public GuideBookWriter NewGuideBookWriter(string id);
	public GuideBookWriter CreateGuideBookWriter();
	public Library NewLibrary(string id);
	public Library CreateLibrary();
	public SpecialistBookWriter NewSpecialistBookWriter(string id);
	public SpecialistBookWriter CreateSpecialistBookWriter();
	public Writer NewWriter(string id);
	public Writer CreateWriter();
}

public class LibraryFactory : AbstractBaseNodeFactory, ILibraryFactory
{
	private readonly LibraryLanguage _language;
	public LibraryFactory(LibraryLanguage language) : base(language)
	{
		_language = language;
	}

	/// <inheritdoc/>
        public override INode CreateNode(string id, Classifier classifier)
	{
		if (_language.Book.EqualsIdentity(classifier))
			return NewBook(id);
		if (_language.GuideBookWriter.EqualsIdentity(classifier))
			return NewGuideBookWriter(id);
		if (_language.Library.EqualsIdentity(classifier))
			return NewLibrary(id);
		if (_language.SpecialistBookWriter.EqualsIdentity(classifier))
			return NewSpecialistBookWriter(id);
		if (_language.Writer.EqualsIdentity(classifier))
			return NewWriter(id);
		throw new UnsupportedClassifierException(classifier);
	}

	/// <inheritdoc/>
        public override Enum GetEnumerationLiteral(EnumerationLiteral literal)
	{
		if (_language.BookType.EqualsIdentity(literal.GetEnumeration()))
			return EnumValueFor<BookType>(literal);
		throw new UnsupportedEnumerationLiteralException(literal);
	}

	/// <inheritdoc/>
        public override IStructuredDataTypeInstance CreateStructuredDataTypeInstance(StructuredDataType structuredDataType, IFieldValues fieldValues)
	{
		throw new UnsupportedStructuredDataTypeException(structuredDataType);
	}

	public virtual Book NewBook(string id) => new(id);
	public virtual Book CreateBook() => NewBook(GetNewId());
	public virtual GuideBookWriter NewGuideBookWriter(string id) => new(id);
	public virtual GuideBookWriter CreateGuideBookWriter() => NewGuideBookWriter(GetNewId());
	public virtual Library NewLibrary(string id) => new(id);
	public virtual Library CreateLibrary() => NewLibrary(GetNewId());
	public virtual SpecialistBookWriter NewSpecialistBookWriter(string id) => new(id);
	public virtual SpecialistBookWriter CreateSpecialistBookWriter() => NewSpecialistBookWriter(GetNewId());
	public virtual Writer NewWriter(string id) => new(id);
	public virtual Writer CreateWriter() => NewWriter(GetNewId());
}

[LionCoreMetaPointer(Language = typeof(LibraryLanguage), Key = "Book")]
public partial class Book : ConceptInstanceBase
{
	private Writer? _author = null;
	/// <remarks>Required Single Reference</remarks>
    	/// <exception cref = "UnsetFeatureException">If Author has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(LibraryLanguage), Key = "author")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Reference, Optional = false, Multiple = false)]
	public Writer Author { get => _author ?? throw new UnsetFeatureException(LibraryLanguage.Instance.Book_author); set => SetAuthor(value); }

	/// <remarks>Required Single Reference</remarks>
        public bool TryGetAuthor([MaybeNullWhenAttribute(false)] out Writer? author)
	{
		author = _author;
		return _author != null;
	}

	/// <remarks>Required Single Reference</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public Book SetAuthor(Writer value)
	{
		AssureNotNull(value, LibraryLanguage.Instance.Book_author);
		_author = value;
		return this;
	}

	private int? _pages = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If Pages has not been set</exception>
        [LionCoreMetaPointer(Language = typeof(LibraryLanguage), Key = "pages")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public int Pages { get => _pages ?? throw new UnsetFeatureException(LibraryLanguage.Instance.Book_pages); set => SetPages(value); }

	/// <remarks>Required Property</remarks>
        public bool TryGetPages([MaybeNullWhenAttribute(false)] out int? pages)
	{
		pages = _pages;
		return _pages != null;
	}

	/// <remarks>Required Property</remarks>
        public Book SetPages(int value)
	{
		_pages = value;
		return this;
	}

	private string? _title = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If Title has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(LibraryLanguage), Key = "title")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public string Title { get => _title ?? throw new UnsetFeatureException(LibraryLanguage.Instance.Book_title); set => SetTitle(value); }

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public bool TryGetTitle([MaybeNullWhenAttribute(false)] out string? title)
	{
		title = _title;
		return _title != null;
	}

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public Book SetTitle(string value)
	{
		AssureNotNull(value, LibraryLanguage.Instance.Book_title);
		_title = value;
		return this;
	}

	private BookType? _type = null;
	/// <remarks>Optional Property</remarks>
        [LionCoreMetaPointer(Language = typeof(LibraryLanguage), Key = "type")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = true, Multiple = false)]
	public BookType? Type { get => _type; set => SetType(value); }

	/// <remarks>Optional Property</remarks>
        public bool TryGetType([MaybeNullWhenAttribute(false)] out BookType? @type)
	{
		@type = _type;
		return _type != null;
	}

	/// <remarks>Optional Property</remarks>
        public Book SetType(BookType? value)
	{
		_type = value;
		return this;
	}

	public Book(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Concept GetConcept() => LibraryLanguage.Instance.Book;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (LibraryLanguage.Instance.Book_author.EqualsIdentity(feature))
		{
			result = Author;
			return true;
		}

		if (LibraryLanguage.Instance.Book_pages.EqualsIdentity(feature))
		{
			result = Pages;
			return true;
		}

		if (LibraryLanguage.Instance.Book_title.EqualsIdentity(feature))
		{
			result = Title;
			return true;
		}

		if (LibraryLanguage.Instance.Book_type.EqualsIdentity(feature))
		{
			result = Type;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (LibraryLanguage.Instance.Book_author.EqualsIdentity(feature))
		{
			if (value is LionWeb.Core.Test.Languages.Generated.V2024_1.Library.M2.Writer v)
			{
				Author = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		if (LibraryLanguage.Instance.Book_pages.EqualsIdentity(feature))
		{
			if (value is int v)
			{
				Pages = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		if (LibraryLanguage.Instance.Book_title.EqualsIdentity(feature))
		{
			if (value is string v)
			{
				Title = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		if (LibraryLanguage.Instance.Book_type.EqualsIdentity(feature))
		{
			if (value is null or LionWeb.Core.Test.Languages.Generated.V2024_1.Library.M2.BookType)
			{
				Type = (LionWeb.Core.Test.Languages.Generated.V2024_1.Library.M2.BookType?)value;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		return false;
	}

	/// <inheritdoc/>
        public override IEnumerable<Feature> CollectAllSetFeatures()
	{
		List<Feature> result = base.CollectAllSetFeatures().ToList();
		if (TryGetAuthor(out _))
			result.Add(LibraryLanguage.Instance.Book_author);
		if (TryGetPages(out _))
			result.Add(LibraryLanguage.Instance.Book_pages);
		if (TryGetTitle(out _))
			result.Add(LibraryLanguage.Instance.Book_title);
		if (TryGetType(out _))
			result.Add(LibraryLanguage.Instance.Book_type);
		return result;
	}
}

[LionCoreMetaPointer(Language = typeof(LibraryLanguage), Key = "GuideBookWriter")]
public partial class GuideBookWriter : Writer
{
	private string? _countries = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If Countries has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(LibraryLanguage), Key = "countries")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public string Countries { get => _countries ?? throw new UnsetFeatureException(LibraryLanguage.Instance.GuideBookWriter_countries); set => SetCountries(value); }

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public bool TryGetCountries([MaybeNullWhenAttribute(false)] out string? countries)
	{
		countries = _countries;
		return _countries != null;
	}

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public GuideBookWriter SetCountries(string value)
	{
		AssureNotNull(value, LibraryLanguage.Instance.GuideBookWriter_countries);
		_countries = value;
		return this;
	}

	public GuideBookWriter(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Concept GetConcept() => LibraryLanguage.Instance.GuideBookWriter;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (LibraryLanguage.Instance.GuideBookWriter_countries.EqualsIdentity(feature))
		{
			result = Countries;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (LibraryLanguage.Instance.GuideBookWriter_countries.EqualsIdentity(feature))
		{
			if (value is string v)
			{
				Countries = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		return false;
	}

	/// <inheritdoc/>
        public override IEnumerable<Feature> CollectAllSetFeatures()
	{
		List<Feature> result = base.CollectAllSetFeatures().ToList();
		if (TryGetCountries(out _))
			result.Add(LibraryLanguage.Instance.GuideBookWriter_countries);
		return result;
	}
}

[LionCoreMetaPointer(Language = typeof(LibraryLanguage), Key = "Library")]
public partial class Library : ConceptInstanceBase
{
	private readonly List<Book> _books = [];
	/// <remarks>Required Multiple Containment</remarks>
    	/// <exception cref = "UnsetFeatureException">If Books is empty</exception>
        [LionCoreMetaPointer(Language = typeof(LibraryLanguage), Key = "books")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Containment, Optional = false, Multiple = false)]
	public IReadOnlyList<Book> Books { get => AsNonEmptyReadOnly(_books, LibraryLanguage.Instance.Library_books); init => AddBooks(value); }

	/// <remarks>Required Multiple Containment</remarks>
        public bool TryGetBooks([MaybeNullWhenAttribute(false)] out IReadOnlyList<Book> books)
	{
		books = _books;
		return _books.Count != 0;
	}

	/// <remarks>Required Multiple Containment</remarks>
    	/// <exception cref = "InvalidValueException">If both Books and nodes are empty</exception>
        public Library AddBooks(IEnumerable<Book> nodes)
	{
		var safeNodes = nodes?.ToList();
		AssureNonEmpty(safeNodes, _books, LibraryLanguage.Instance.Library_books);
		_books.AddRange(SetSelfParent(safeNodes, LibraryLanguage.Instance.Library_books));
		return this;
	}

	/// <remarks>Required Multiple Containment</remarks>
    	/// <exception cref = "InvalidValueException">If both Books and nodes are empty</exception>
    	/// <exception cref = "ArgumentOutOfRangeException">If index negative or greater than Books.Count</exception>
        public Library InsertBooks(int index, IEnumerable<Book> nodes)
	{
		AssureInRange(index, _books);
		var safeNodes = nodes?.ToList();
		AssureNonEmpty(safeNodes, _books, LibraryLanguage.Instance.Library_books);
		AssureNoSelfMove(index, safeNodes, _books);
		_books.InsertRange(index, SetSelfParent(safeNodes, LibraryLanguage.Instance.Library_books));
		return this;
	}

	/// <remarks>Required Multiple Containment</remarks>
    	/// <exception cref = "InvalidValueException">If Books would be empty</exception>
        public Library RemoveBooks(IEnumerable<Book> nodes)
	{
		var safeNodes = nodes?.ToList();
		AssureNotNull(safeNodes, LibraryLanguage.Instance.Library_books);
		AssureNotClearing(safeNodes, _books, LibraryLanguage.Instance.Library_books);
		RemoveSelfParent(safeNodes, _books, LibraryLanguage.Instance.Library_books);
		return this;
	}

	private string? _name = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If Name has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(LibraryLanguage), Key = "library_Library_name")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public string Name { get => _name ?? throw new UnsetFeatureException(LibraryLanguage.Instance.Library_name); set => SetName(value); }

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public bool TryGetName([MaybeNullWhenAttribute(false)] out string? name)
	{
		name = _name;
		return _name != null;
	}

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public Library SetName(string value)
	{
		AssureNotNull(value, LibraryLanguage.Instance.Library_name);
		_name = value;
		return this;
	}

	public Library(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Concept GetConcept() => LibraryLanguage.Instance.Library;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (LibraryLanguage.Instance.Library_books.EqualsIdentity(feature))
		{
			result = Books;
			return true;
		}

		if (LibraryLanguage.Instance.Library_name.EqualsIdentity(feature))
		{
			result = Name;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (LibraryLanguage.Instance.Library_books.EqualsIdentity(feature))
		{
			var enumerable = LibraryLanguage.Instance.Library_books.AsNodes<LionWeb.Core.Test.Languages.Generated.V2024_1.Library.M2.Book>(value).ToList();
			AssureNonEmpty(enumerable, LibraryLanguage.Instance.Library_books);
			RemoveSelfParent(_books.ToList(), _books, LibraryLanguage.Instance.Library_books);
			AddBooks(enumerable);
			return true;
		}

		if (LibraryLanguage.Instance.Library_name.EqualsIdentity(feature))
		{
			if (value is string v)
			{
				Name = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		return false;
	}

	/// <inheritdoc/>
        public override IEnumerable<Feature> CollectAllSetFeatures()
	{
		List<Feature> result = base.CollectAllSetFeatures().ToList();
		if (TryGetBooks(out _))
			result.Add(LibraryLanguage.Instance.Library_books);
		if (TryGetName(out _))
			result.Add(LibraryLanguage.Instance.Library_name);
		return result;
	}

	/// <inheritdoc/>
        protected override bool DetachChild(INode child)
	{
		if (base.DetachChild(child))
			return true;
		Containment? c = GetContainmentOf(child);
		if (LibraryLanguage.Instance.Library_books.EqualsIdentity(c))
		{
			RemoveSelfParent(child, _books, LibraryLanguage.Instance.Library_books);
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        public override Containment? GetContainmentOf(INode child)
	{
		Containment? result = base.GetContainmentOf(child);
		if (result != null)
			return result;
		if (child is Book child0 && _books.Contains(child0))
			return LibraryLanguage.Instance.Library_books;
		return null;
	}
}

[LionCoreMetaPointer(Language = typeof(LibraryLanguage), Key = "SpecialistBookWriter")]
public partial class SpecialistBookWriter : Writer
{
	private string? _subject = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If Subject has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(LibraryLanguage), Key = "subject")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public string Subject { get => _subject ?? throw new UnsetFeatureException(LibraryLanguage.Instance.SpecialistBookWriter_subject); set => SetSubject(value); }

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public bool TryGetSubject([MaybeNullWhenAttribute(false)] out string? subject)
	{
		subject = _subject;
		return _subject != null;
	}

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public SpecialistBookWriter SetSubject(string value)
	{
		AssureNotNull(value, LibraryLanguage.Instance.SpecialistBookWriter_subject);
		_subject = value;
		return this;
	}

	public SpecialistBookWriter(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Concept GetConcept() => LibraryLanguage.Instance.SpecialistBookWriter;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (LibraryLanguage.Instance.SpecialistBookWriter_subject.EqualsIdentity(feature))
		{
			result = Subject;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (LibraryLanguage.Instance.SpecialistBookWriter_subject.EqualsIdentity(feature))
		{
			if (value is string v)
			{
				Subject = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		return false;
	}

	/// <inheritdoc/>
        public override IEnumerable<Feature> CollectAllSetFeatures()
	{
		List<Feature> result = base.CollectAllSetFeatures().ToList();
		if (TryGetSubject(out _))
			result.Add(LibraryLanguage.Instance.SpecialistBookWriter_subject);
		return result;
	}
}

[LionCoreMetaPointer(Language = typeof(LibraryLanguage), Key = "Writer")]
public partial class Writer : ConceptInstanceBase
{
	private string? _name = null;
	/// <remarks>Required Property</remarks>
    	/// <exception cref = "UnsetFeatureException">If Name has not been set</exception>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        [LionCoreMetaPointer(Language = typeof(LibraryLanguage), Key = "library_Writer_name")]
	[LionCoreFeature(Kind = LionCoreFeatureKind.Property, Optional = false, Multiple = false)]
	public string Name { get => _name ?? throw new UnsetFeatureException(LibraryLanguage.Instance.Writer_name); set => SetName(value); }

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public bool TryGetName([MaybeNullWhenAttribute(false)] out string? name)
	{
		name = _name;
		return _name != null;
	}

	/// <remarks>Required Property</remarks>
    	/// <exception cref = "InvalidValueException">If set to null</exception>
        public Writer SetName(string value)
	{
		AssureNotNull(value, LibraryLanguage.Instance.Writer_name);
		_name = value;
		return this;
	}

	public Writer(string id) : base(id)
	{
	}

	/// <inheritdoc/>
        public override Concept GetConcept() => LibraryLanguage.Instance.Writer;
	/// <inheritdoc/>
        protected override bool GetInternal(Feature? feature, out Object? result)
	{
		if (base.GetInternal(feature, out result))
			return true;
		if (LibraryLanguage.Instance.Writer_name.EqualsIdentity(feature))
		{
			result = Name;
			return true;
		}

		return false;
	}

	/// <inheritdoc/>
        protected override bool SetInternal(Feature? feature, Object? value)
	{
		if (base.SetInternal(feature, value))
			return true;
		if (LibraryLanguage.Instance.Writer_name.EqualsIdentity(feature))
		{
			if (value is string v)
			{
				Name = v;
				return true;
			}

			throw new InvalidValueException(feature, value);
		}

		return false;
	}

	/// <inheritdoc/>
        public override IEnumerable<Feature> CollectAllSetFeatures()
	{
		List<Feature> result = base.CollectAllSetFeatures().ToList();
		if (TryGetName(out _))
			result.Add(LibraryLanguage.Instance.Writer_name);
		return result;
	}
}

[LionCoreMetaPointer(Language = typeof(LibraryLanguage), Key = "BookType")]
public enum BookType
{
	[LionCoreMetaPointer(Language = typeof(LibraryLanguage), Key = "Normal")]
	Normal,
	[LionCoreMetaPointer(Language = typeof(LibraryLanguage), Key = "Special")]
	Special
}