import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { languages } from '../localization';
import { Select, MenuItem, FormControl, InputLabel } from '@mui/material';
import { getCookie, setCookie } from './helpers';

const LANGUAGE_COOKIE = "user_language";
const COOKIE_EXP_DAYS = 365;

const LanguageSelector = () => {
  const { t, i18n } = useTranslation();

  useMemo(() => {
    const saved = getCookie(LANGUAGE_COOKIE);
    if (saved && saved !== i18n.language) {
      i18n.changeLanguage(saved);
    }
  }, [i18n]);

  const changeLanguage = (lng: string) => {
    i18n.changeLanguage(lng);
    setCookie(LANGUAGE_COOKIE, lng, COOKIE_EXP_DAYS);
  };

  const orderedLanguages = useMemo(
    () => [i18n.language, ...languages.filter(lng => lng !== i18n.language)],
    [i18n.language]
  );

  return (
    <FormControl size="small" variant="outlined">
      <InputLabel id="language-selector-label" sx={{ color: 'inherit' }}>{t('General.language')}</InputLabel>
      <Select
        sx={{ color: 'inherit' }}
        labelId="language-selector-label"
        value={i18n.language}
        label={t('General.language')}
        onChange={(e) => changeLanguage(e.target.value)}
      >
        {orderedLanguages.map((lng) => {
          const lngTitle = i18n.getFixedT(lng)('CountryName');
          return <MenuItem key={lng} value={lng}>
            {lngTitle}
          </MenuItem>
        })}
      </Select>
    </FormControl>
  );
};

export default LanguageSelector;
