param (
    $Version
)

docker build --rm -t loremfoobar/reportgenerator-coverage-bitbucket-pipe:$Version .
