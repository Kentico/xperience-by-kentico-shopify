import { test, expect, APIRequestContext } from "@playwright/test";
import { CheckoutPage } from "../pageObjects/CheckoutPage";
import { BasePage } from "../pageObjects/BasePage";

const SHOPIFY_ACCESS_TOKEN = process.env.CMSSHOPIFYCONFIG__ADMINAPIKEY;
const SHOPIFY_URL = process.env.CMSSHOPIFYCONFIG__SHOPIFYURL;
const SHOPIFY_STORE_PASSWORD = process.env.CMSSHOPIFYCONFIG__STOREPASSWORD;

test.describe("Orders", () => {
  let basePage: BasePage;
  let apiContext: APIRequestContext;

  test.beforeAll(async ({ playwright }) => {
    if (!SHOPIFY_ACCESS_TOKEN || !SHOPIFY_URL || !SHOPIFY_STORE_PASSWORD) {
      throw new Error("Invalid env variable");
    }
    apiContext = await playwright.request.newContext({
      baseURL: `${SHOPIFY_URL}/admin/api/2024-10/`,
      extraHTTPHeaders: {
        "X-Shopify-Access-Token": SHOPIFY_ACCESS_TOKEN,
      },
    });
  });

  test.afterAll(async ({}) => {
    await apiContext.dispose();
  });

  test.beforeEach(async ({ page }) => {
    basePage = new BasePage(page);
    await basePage.goto("/store");
  });

  test(`Complete order with updated quantity`, async ({ page }) => {
    const testData = {
      customer: {
        firstName: "Testfirstname",
        lastName: "Testlastname",
        email: "autotest@test.cz",
        address: "Botanická 42/99",
        city: "Brno",
        psc: "602 00",
        country: "Czechia",
      },
      shipping: "Standard",
      payment: {
        cardNumber: "1",
        cardExpirationDate: "12/27",
        cardSecurityCode: "123",
      },
    };
    test.info().annotations.push({
      type: "Info",
      description: JSON.stringify(testData),
    });

    await test.step("Select category 'Brewing kits' from store page", async () => {
      await page.locator(".store-menu-list li", { hasText: "Brewing kits" }).click();
      await expect(page).toHaveURL("/store/brewing-kits");
    });
    await test.step(`Select product 'Aeropress'`, async () => {
      await page.locator(".product-tile", { hasText: "Aeropress" }).click();
      await expect(page.locator(".product-detail", { hasText: "Aeropress" })).toBeVisible(); // expecting to be on product page of Coffee Plunger
    });
    await test.step("Add product to cart", async () => {
      await page.locator("button", { hasText: "Add to cart" }).click();
    });
    await test.step("Go to shopping cart", async () => {
      await page.goto("/shopping-cart");
    });
    await test.step("Update quantity", async () => {
      await page.locator('input[name="Quantity"]').fill("2");
      await page.locator('input[name="cartOperation"][value="Update"]').click();
      await expect(page.locator(".cart-item-info__price")).toContainText(`1884 Kč`);
      await expect(page.locator(".cart-total")).toContainText(`1884 Kč`);
    });
    await test.step("Go to checkout", async () => {
      await page.locator("a", { hasText: "Go to shopify checkout page" }).click();
    });
    await test.step("Login to shopify store", async () => {
      if (!SHOPIFY_STORE_PASSWORD) {
        throw new Error("Invalid env variable SHOPIFY_STORE_PASSWORD");
      }
      await page.locator("#password").fill(SHOPIFY_STORE_PASSWORD);
      await page.locator('button[type="submit"]').click();
      await page.goto("/shopping-cart");
      await page.locator("a", { hasText: "Go to shopify checkout page" }).click();
    });

    const checkoutPage = new CheckoutPage(page);

    await test.step("Fill customer details, shipping and payment information", async () => {
      await checkoutPage.fillCustomerDetails(testData.customer);
      await expect(page.locator('[role="row"]', { hasText: /Shipping/ })).toContainText("Kč 30.00"); // implicit option, also automatically waits for the contact details to be processed and shipping options visible
      await checkoutPage.selectShipping(testData.shipping);
      await expect(page.locator('[role="row"]', { hasText: /Shipping/ })).toContainText("Kč 450.00"); // selected option
      await checkoutPage.fillPayment(testData.payment);
    });
    await test.step("Check calculated price", async () => {
      await expect(page.locator('[role="row"]', { hasText: /Total/ })).toContainText("Kč 2,334.00"); // 2x Aeropress + shipping (450czk)
    });

    let orderId: string;

    await test.step("Confirm order and check thank you page", async () => {
      await checkoutPage.confirmOrder();
      await expect(page.locator("#checkout-main")).toContainText("Thank you");
      await expect(page.locator('[role="row"]', { hasText: /Total/ })).toContainText("Kč 2,334.00");
      await page.waitForTimeout(3000); // explicit wait for shopify to process
      await page.locator("a", { hasText: "Continue shopping" }).click();
      await expect(page).toHaveURL(/\/thank-you\?orderId=[0-9]*/);
      await expect(page.locator(".thank-you-content")).toContainText("Thank You");
      orderId = await page.url().split("orderId=")[1];
    });

    await test.step("Check order in shopify API", async () => {
      const response = await apiContext.get(`./orders/${orderId}.json`, {
        maxRedirects: 0,
      });
      test.info().annotations.push({
        type: "Received HTTP code of shopify order",
        description: response.status() + " " + response.statusText(),
      });
      expect(response.ok()).toBeTruthy();
      const apiOrder = await response.json();
      expect(apiOrder).toEqual(
        expect.objectContaining({
          order: expect.objectContaining({
            contact_email: testData.customer.email,
            billing_address: expect.objectContaining({
              first_name: testData.customer.firstName,
              address1: testData.customer.address,
              phone: null,
              city: testData.customer.city,
              zip: testData.customer.psc,
              country: "Czech Republic",
              last_name: testData.customer.lastName,
              country_code: "CZ",
            }),
            shipping_address: expect.objectContaining({
              first_name: testData.customer.firstName,
              address1: testData.customer.address,
              phone: null,
              city: testData.customer.city,
              zip: testData.customer.psc,
              country: "Czech Republic",
              last_name: testData.customer.lastName,
              country_code: "CZ",
            }),
            current_total_price_set: expect.objectContaining({
              presentment_money: {
                amount: "2334.00",
                currency_code: "CZK",
              },
            }),
            total_price_set: expect.objectContaining({
              presentment_money: {
                amount: "2334.00",
                currency_code: "CZK",
              },
            }),
            line_items: [
              expect.objectContaining({
                gift_card: false,
                grams: 700,
                name: "Aeropress",
                price_set: expect.objectContaining({
                  presentment_money: {
                    amount: "942.00",
                    currency_code: "CZK",
                  },
                }),
                quantity: 2,
                sku: "SK1U",
                taxable: true,
                title: "Aeropress",
                total_discount: "0.00",
                total_discount_set: expect.objectContaining({
                  presentment_money: {
                    amount: "0.00",
                    currency_code: "CZK",
                  },
                }),
              }),
            ],
            shipping_lines: [
              expect.objectContaining({
                code: "Standard",
                price_set: expect.objectContaining({
                  presentment_money: {
                    amount: "450.00",
                    currency_code: "CZK",
                  },
                }),
              }),
            ],
          }),
        })
      );
    });
  });
});
