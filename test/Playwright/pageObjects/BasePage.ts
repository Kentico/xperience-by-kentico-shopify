import { type Locator, type Page } from "@playwright/test";

export class BasePage {
  readonly page: Page;

  constructor(page: Page) {
    this.page = page;
  }

  async goto(url: string) {
    await this.page.goto(url);
    await this.fullyLoadPage();
  }
  async fullyLoadPage() {
    await this.page.evaluate(() =>
      document.querySelectorAll("img[loading=lazy]").forEach((img) => img.setAttribute("loading", "eager"))
    );

    await this.page.evaluate(async () => {
      await new Promise((resolve) => {
        let totalHeight = 0;
        const distance = 200;
        const timer = setInterval(() => {
          const scrollHeight = document.body.scrollHeight;
          window.scrollBy(0, distance);
          totalHeight += distance;
          if (totalHeight >= scrollHeight - window.innerHeight) {
            clearInterval(timer);
            resolve(void 0);
          }
        }, 70);
      });
      window.scrollTo(0, 0);
    });
  }
}
