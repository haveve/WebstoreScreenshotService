import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';

import translationEN from './localization/en.json';
import translationUK from './localization/uk.json';

const en = "en";
const ua = "ua";

export const MatchLocateToLanguage = (locale: string | null | undefined) => {
  switch (locale?.toLowerCase()) {
    case "en-us":
      return en;
    case "uk-ua":
      return ua;
    default:
      return en;
  }
}

export const DEFAULT_LOCALE = en;
export const SUPPORTED_LOCALES = [en, ua] as const;

export type Locale = typeof SUPPORTED_LOCALES[number];

export const isValidLocale = (lng?: string): lng is Locale =>
  !!lng && SUPPORTED_LOCALES.includes(lng as Locale);

const resources = {
  [en]: {
    translation: translationEN
  },
  [ua]: {
    translation: translationUK
  }
};

export default i18n
  .use(initReactI18next)
  .init({
    resources,
    lng: DEFAULT_LOCALE,
    fallbackLng: DEFAULT_LOCALE,
    supportedLngs: SUPPORTED_LOCALES,
    load: "languageOnly",
    keySeparator: '.',
    interpolation: {
      escapeValue: false
    }
  });;