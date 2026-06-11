import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import en from "./locales/en.json";
import ptBR from "./locales/pt-BR.json";

export const supportedLanguages = ["pt-BR", "en"] as const;
export type SupportedLanguage = (typeof supportedLanguages)[number];

export const fallbackLanguage: SupportedLanguage = "pt-BR";
export const languageStorageKey = "convertly.language";

function getInitialLanguage(): SupportedLanguage {
  const storedLanguage = window.localStorage.getItem(languageStorageKey);
  if (isSupportedLanguage(storedLanguage)) {
    return storedLanguage;
  }

  const browserLanguage = window.navigator.language;
  if (browserLanguage.toLowerCase().startsWith("pt")) {
    return "pt-BR";
  }

  if (browserLanguage.toLowerCase().startsWith("en")) {
    return "en";
  }

  return fallbackLanguage;
}

export function isSupportedLanguage(language: string | null | undefined): language is SupportedLanguage {
  return supportedLanguages.includes(language as SupportedLanguage);
}

void i18n.use(initReactI18next).init({
  resources: {
    "pt-BR": { translation: ptBR },
    en: { translation: en },
  },
  lng: getInitialLanguage(),
  fallbackLng: fallbackLanguage,
  interpolation: {
    escapeValue: false,
  },
  returnEmptyString: false,
});

i18n.on("languageChanged", (language) => {
  if (isSupportedLanguage(language)) {
    window.localStorage.setItem(languageStorageKey, language);
  }
});

export { i18n };
