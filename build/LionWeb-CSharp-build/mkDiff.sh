cd chunks
 
function cleanUp() {
  find . -regex '.*\.sorted.json' | xargs rm
  find . -regex '.*\.txt' | xargs rm
}

cleanUp


LIONWEB_CLI="@lionweb/cli"

npx $LIONWEB_CLI sort externalDefs/lioncore.json localDefs/lioncore.json

npx $LIONWEB_CLI textualize localDefs/shapesLanguage.json
npx $LIONWEB_CLI textualize externalDefs/lioncore.json localDefs/lioncore.json

diff externalDefs/lioncore.sorted.json localDefs/lioncore.sorted.json \
  && cleanUp
    # only leave extracted files when there's some diff

cd ..

