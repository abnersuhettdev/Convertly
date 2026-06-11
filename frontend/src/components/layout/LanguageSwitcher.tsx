import { Languages } from "lucide-react";
import { useTranslation } from "react-i18next";
import { isSupportedLanguage, supportedLanguages, type SupportedLanguage } from "../../i18n";

export function LanguageSwitcher() {
  const { i18n, t } = useTranslation();
  const currentLanguage = isSupportedLanguage(i18n.resolvedLanguage) ? i18n.resolvedLanguage : "pt-BR";

  async function handleChange(language: SupportedLanguage) {
    await i18n.changeLanguage(language);
  }

  return (
    <div
      aria-label={t("language.label")}
      className="inline-flex items-center gap-1 rounded-full border border-slate-200/80 bg-white/85 p-1 shadow-sm shadow-slate-900/5 backdrop-blur"
      role="group"
    >
      <Languages aria-hidden="true" className="ml-2 h-4 w-4 text-slate-500" />
      {supportedLanguages.map((language) => (
        <button
          aria-label={`${t("language.label")}: ${t(`language.${language === "pt-BR" ? "ptBR" : "en"}`)}`}
          aria-pressed={currentLanguage === language}
          className={`h-8 rounded-full px-2.5 text-xs font-semibold transition focus-visible:outline focus-visible:outline-2 focus-visible:outline-emerald-600 ${
            currentLanguage === language ? "bg-slate-950 text-white shadow-sm" : "text-slate-600 hover:bg-slate-100 hover:text-slate-950"
          }`}
          key={language}
          onClick={() => void handleChange(language)}
          type="button"
        >
          {t(`language.${language === "pt-BR" ? "ptBR" : "en"}`)}
        </button>
      ))}
    </div>
  );
}
