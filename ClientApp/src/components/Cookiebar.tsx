import React from 'react';
import { useDispatch } from 'react-redux';
import { useAppSelector } from '../behavior/rootReducer';
import cookieStore from '../behavior/cookie/store';
import { getLogoutAction } from '../behavior/epic';
import { useTranslation } from 'react-i18next';
import { Box, Button, Typography, Link } from '@mui/material';

type Props = {
    showCookieBar: boolean;
    setVisibility: (visible: boolean) => void;
}

const CookieBar = ({ showCookieBar, setVisibility }: Props) => {
    const { t } = useTranslation();
    const user = useAppSelector(state => state.basic.user);
    const dispatch = useDispatch();

    const handleChoice = (choice: boolean) => {
        cookieStore.setCookieConsent(choice);
        setVisibility(false);
        if (!choice && user) {
            dispatch(getLogoutAction());
        }
    };

    if (!showCookieBar) return null;

    return (
        <Box
            sx={{
                position: 'fixed',
                bottom: 0,
                left: 0,
                width: '100%',
                bgcolor: 'grey.900',
                color: 'common.white',
                display: 'flex',
                justifyContent: 'space-between',
                alignItems: 'center',
                p: 2,
                zIndex: 1300,
                flexWrap: 'wrap',
                gap: 2,
            }}
        >
            <Typography variant="body1" sx={{ flex: 1, textAlign: 'center' }}>
                {t('CookieBar.cookieMessage')}
                <Link href="/privacy-policy" color="secondary" underline="hover">
                    {t('CookieBar.privacyPolicy')}
                </Link>
            </Typography>
            <Box sx={{ display: 'flex', gap: 1 }}>
                <Button
                    variant="contained"
                    color="success"
                    onClick={() => handleChoice(true)}
                >
                    {t('CookieBar.acceptCookies')}
                </Button>
                <Button
                    variant="contained"
                    color="error"
                    onClick={() => handleChoice(false)}
                >
                    {t('CookieBar.decline')}
                </Button>
            </Box>
        </Box>
    );
};

export default CookieBar;
