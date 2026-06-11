import { useMemo } from 'react';
import { useTranslation } from 'react-i18next';
import { isValidLocale, SUPPORTED_LOCALES } from '../localization';
import { Select, MenuItem, FormControl, InputLabel } from '@mui/material';
import { useLocation, useNavigate } from 'react-router-dom';

const LanguageSelector = () => {
  const { t, i18n } = useTranslation();
  const location = useLocation();
  const navigate = useNavigate();

  const changeLanguage = (newLocale: string) => {
    const segments = location.pathname.split("/").filter(Boolean);

    const currentLocale = isValidLocale(segments[0]) ? segments[0] : null;

    const pathWithoutLocale = currentLocale
      ? "/" + segments.slice(1).join("/")
      : location.pathname;

    // const newPath =
    //   newLocale === "en-us"
    //     ? pathWithoutLocale
    //     : `/${newLocale}${pathWithoutLocale}`;

    i18n.changeLanguage(newLocale);
    //navigate(newPath);
  };

  const orderedLanguages = useMemo(
    () => [i18n.language, ...SUPPORTED_LOCALES.filter(lng => lng !== i18n.language)],
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
