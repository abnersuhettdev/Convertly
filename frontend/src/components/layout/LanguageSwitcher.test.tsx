import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { beforeEach, describe, expect, it } from "vitest";
import "../../i18n";
import { i18n, languageStorageKey } from "../../i18n";
import { LanguageSwitcher } from "./LanguageSwitcher";

describe("LanguageSwitcher", () => {
  beforeEach(async () => {
    window.localStorage.clear();
    await i18n.changeLanguage("en");
  });

  it("exposes language options with accessible names and saves the preference", async () => {
    const user = userEvent.setup();
    render(<LanguageSwitcher />);

    expect(screen.getByRole("group", { name: "Select language" })).toBeInTheDocument();
    expect(screen.getByRole("button", { name: "Select language: EN" })).toHaveAttribute("aria-pressed", "true");

    await user.click(screen.getByRole("button", { name: "Select language: PT-BR" }));

    expect(window.localStorage.getItem(languageStorageKey)).toBe("pt-BR");
    expect(screen.getByRole("button", { name: "Selecionar idioma: PT-BR" })).toHaveAttribute("aria-pressed", "true");
  });
});
