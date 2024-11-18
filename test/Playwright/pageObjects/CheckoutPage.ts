import { type Locator, type Page } from "@playwright/test";
import { BasePage } from "./BasePage";

export class CheckoutPage extends BasePage {
  readonly $firstName: Locator;
  readonly $lastName: Locator;
  readonly $email: Locator;
  readonly $address: Locator;
  readonly $city: Locator;
  readonly $psc: Locator;
  readonly $country: Locator;
  readonly $shippingMethods: Locator;
  readonly $cardNumber: Locator;
  readonly $cardExpirationDate: Locator;
  readonly $cardSecurityCode: Locator;
  readonly $payBtn: Locator;

  constructor(page: Page) {
    super(page);

    this.$firstName = page.locator('#TextField0[name="firstName"]');
    this.$lastName = page.locator('#TextField1[name="lastName"]');
    this.$email = page.locator('input[name="email"]');
    this.$address = page.locator('#TextField2[name="address1"]');
    this.$city = page.locator('#TextField5[name="city"]');
    this.$psc = page.locator('#TextField4[name="postalCode"]');
    this.$country = page.locator('select[name="countryCode"]');
    this.$shippingMethods = page.locator("#shipping_methods");
    this.$cardNumber = page
      .frameLocator('iframe[title="Field container for: Card number"]')
      .getByPlaceholder("Card number");
    this.$cardExpirationDate = page
      .frameLocator('iframe[title="Field container for: Expiration date (MM / YY)"]')
      .getByPlaceholder("Expiration date (MM / YY)");
    this.$cardSecurityCode = page
      .frameLocator('iframe[title="Field container for: Security code"]')
      .getByPlaceholder("Security code");
    this.$payBtn = page.locator("#checkout-pay-button");
  }

  async fillCustomerDetails(details) {
    await this.$firstName.fill(details.firstName);
    await this.$lastName.fill(details.lastName);
    await this.$email.fill(details.email);
    await this.$address.fill(details.address);
    await this.$psc.fill(details.psc);
    await this.$city.fill(details.city);
    await this.$country.selectOption({ label: details.country });
  }
  async selectShipping(shipping) {
    await this.$shippingMethods.locator("label", { hasText: shipping }).click();
  }
  async fillPayment(payment) {
    await this.$cardNumber.fill(payment.cardNumber);
    await this.$cardExpirationDate.fill(payment.cardExpirationDate);
    await this.$cardSecurityCode.fill(payment.cardSecurityCode);
  }
  async confirmOrder() {
    await this.$payBtn.click();
  }
}
