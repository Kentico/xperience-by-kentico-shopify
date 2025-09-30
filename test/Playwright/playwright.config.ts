import { defineConfig, devices } from "@playwright/test";

/**
 * Read environment variables from file.
 * https://github.com/motdotla/dotenv
 */
// import dotenv from 'dotenv';
// import path from 'path';
// dotenv.config({ path: path.resolve(__dirname, '.env') });

/**
 * See https://playwright.dev/docs/test-configuration.
 */

const XBYK_STORE_URL = process.env.ASPNETCORE_URLS;

export default defineConfig({
  testDir: "./tests",
  snapshotPathTemplate: `{testDir}/screenshots/{projectName}/{testFilePath}/{arg}{ext}`,
  /* Run tests in files in parallel */
  fullyParallel: true,
  /* Fail the build on CI if you accidentally left test.only in the source code. */
  forbidOnly: !!process.env.CI,
  retries: 1,
  /* Opt out of parallel tests on CI. */
  workers: 4,
  /* Reporter to use. See https://playwright.dev/docs/test-reporters */
  reporter: [["list"], ["html", { open: "never" }], ["junit", { outputFile: "test-results/e2e-junit-results.xml" }]],
  /* Shared settings for all the projects below. See https://playwright.dev/docs/api/class-testoptions. */
  use: {
    screenshot: { mode: "only-on-failure", fullPage: true },
    video: "retain-on-failure",
    /* Collect trace when retrying the failed test. See https://playwright.dev/docs/trace-viewer */
    trace: "on-first-retry",
    /* Base URL to use in actions like `await page.goto('/')`. */
    // baseURL: 'http://127.0.0.1:3000',
    actionTimeout: 40_000,
    launchOptions: {
      slowMo: 250,
    },
  },
  timeout: 60 * 1000, // Maximum time a test can run
  expect: {
    timeout: 40_000, // Maximum time expect() should wait for the condition to be met.
  },

  /* Configure projects for major browsers */
  projects: [
    {
      name: "chromium",
      use: {
        ...devices["Desktop Chrome"],
        baseURL: XBYK_STORE_URL,
        ignoreHTTPSErrors: true,
      },
    },

    // {
    //   name: "firefox",
    //   use: { ...devices["Desktop Firefox"], baseURL: "https://localhost:14066/", ignoreHTTPSErrors: true },
    // },

    // {
    //   name: "webkit",
    //   use: { ...devices["Desktop Safari"], baseURL: "https://localhost:14066/", ignoreHTTPSErrors: true },
    // },

    /* Test against mobile viewports. */
    // {
    //   name: 'Mobile Chrome',
    //   use: { ...devices['Pixel 5'] },
    // },
    // {
    //   name: 'Mobile Safari',
    //   use: { ...devices['iPhone 12'] },
    // },

    /* Test against branded browsers. */
    // {
    //   name: 'Microsoft Edge',
    //   use: { ...devices['Desktop Edge'], channel: 'msedge' },
    // },
    // {
    //   name: 'Google Chrome',
    //   use: { ...devices['Desktop Chrome'], channel: 'chrome' },
    // },
  ],

  /* Run your local dev server before starting the tests */
  // webServer: {
  //   command: 'npm run start',
  //   url: 'http://127.0.0.1:3000',
  //   reuseExistingServer: !process.env.CI,
  // },
});
