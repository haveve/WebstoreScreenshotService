import i18n from 'i18next';
import { initReactI18next } from 'react-i18next';

import translationEN from './localization/en.json';
import translationUK from './localization/uk.json';

const resources = {
  en: {
    translation: translationEN
  },
  uk: {
    translation: translationUK
  }
};

export const languages = Object.keys(resources);

export default i18n
.use(initReactI18next)
.init({
  resources,
  lng: 'en',
  fallbackLng: 'en', 
  keySeparator: '.',
  interpolation: {
    escapeValue: false
  }
});;