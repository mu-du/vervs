# Versioning on Viusal Studio Application (vervs.exe)

## Introduction

Update version number on the files:

* Visual Studio 2017, 2019 or 2020 project files including sdk, sdk.web, and sdk.worker
* NuGet version on nuspec files, and version of dependencies
* Version number defined on AssemblyInfo.cs

## Usage

* vervs &lt;version-number&gt; &lt;build-source-directory&gt;

* vervs &lt;version-number&gt; &lt;repo-name&gt;

  Note: Setup environment variable [GitHubHome] if repo-name used.

## Examples

* vervs 1.0.9.0 c:\devel\GitHub\vervs

* vervs 1.0.9.0 sqlcode

* vervs 1.0.9.0
