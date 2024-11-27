# Playwright E2E tests

## Description

E2E tests are implemented using Playwright - an open-source, NodeJS-based framework for web testing and automation.

To prevent false-positives, tests are configured to run maximum of 2 times based on the result of the 1st test. Following combinations apply.

- test PASSES the 1st run -> Test **PASSED**
- test FAILS the 1st run, retries and PASSES the 2nd run -> Test **PASSED** with **FLAKY** signature
- test FAILS the 1st, retries and FAILS the 2nd run -> Test **FAILED**

## Install dependencies

`npm i`

`npx playwright install --with-deps`

## Run tests

Following env variables are expected to be set

- ASPNETCORE_URLS = Base url of the app
- CMSSHOPIFYCONFIG\_\_ADMINAPIKEY = Shopify API access token
- CMSSHOPIFYCONFIG\_\_SHOPIFYURL = Shopify store URL
- CMSSHOPIFYCONFIG\_\_STOREPASSWORD = Shopify store password

Run the tests with command

`npx playwright test`

## Test results

Test results are generated whether the tests pass or fail.

#### HTML

HTML test result can be opened using

`npx playwright show-report`

#### JUnit

Test results are generated also in JUnit format, for possible integration with other reporting systems. XML file can be found in `test-results/e2e-junit-results.xml`

#### Artifacts

In case of a test failure, following artifacts are created

- Screenshot of the last application state (point of failure) - generates on every fail
- Video of full test run - generates on every fail
- Trace - generates on first retry

Trace contains enhanced information to ease debugging failures. It can be opened with Trace Viewer (GUI tool) locally or using hosted variant by Playwright where it is possible to upload trace files using drag and drop.

To open local Trace Viewer use

`npx playwright show-trace path/to/trace.zip`

Hosted variant by Playwright can be found at

[trace.playwright.dev](https://trace.playwright.dev/)
