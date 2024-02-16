<h1 align="center">
  <a href="https://github.com/orangebeard-io/dotnet-client">
    <img src="https://raw.githubusercontent.com/orangebeard-io/dotnet-client/master/.github/logo.svg" alt="Orangebeard.io .Net Client" height="200">
  </a>
  <br>Orangebeard.io .Net Client<br>
</h1>

<h4 align="center">Orangebeard .Net client for .Net based Orangebeard Listeners.</h4>

<p align="center">
  <a href="https://github.com/orangebeard-io/java-client/actions">
    <img src="https://img.shields.io/github/workflow/status/orangebeard-io/java-client/release?style=flat-square"
      alt="Build Status" />
  </a>
  <a href="https://github.com/orangebeard-io/java-client/blob/master/LICENSE">
    <img src="https://img.shields.io/github/license/orangebeard-io/dotnet-client?style=flat-square"
      alt="License" />
  </a>
</p>

<div align="center">
  <h4>
    <a href="https://orangebeard.io">Orangebeard</a> |
    <a href="#installation">Installation</a>
  </h4>
</div>

## Installation

### Build/Download the dll

### Set configuration parameters:

- *orangebeard.endpoint* - You Orangebeard API Url
- *orangebeard.project* - The project to report to
- *orangebeard.testset* - The testset name to report
- *orangebeard.description* - [Optional] The description for the test run
- *orangebeard.attributes* - [Optional] Attributes to set to the test run
- *orangebeard.accessToken* - Your API access token
- *orangebeard.fileupload.patterns* - [Optional] A semicolon-separated list of regular expressions to match when
  deciding to upload file references from log entries as attachments
- *orangebeard.finishtestruntimeout* - [Optional, default: 60] The max number of seconds to wait for all Orangebeard
  calls to be processed. 