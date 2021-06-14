# Contributing

## Conventional Commits

Please follow [Conventional Commits](https://www.conventionalcommits.org/)
format for all commit messages.

## Deploy to Docker Hub

### Version Release Commit

Make sure to separate version release from other changes. See past
release commits for reference.

### Tagging

The Docker Hub repository has automatic build enabled for tags push.
Tags in the format v1.2.3 are permanent tags and should never be moved.
For convenience, a feature tag should represent the latest patch version
of a feature version, with the format v1.2. For example, when releasing
v1.5.0, also create tag v1.5 on same commit. Then, when releasing v1.5.1,
move tag v1.5 to the same commit as v1.5.1.
