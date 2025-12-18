import React, { useEffect } from 'react';
import { useAppSelector } from '../behavior/rootReducer';
import { useDispatch } from 'react-redux';
import { getLoginAction } from '../behavior/epic';
import { useNavigate } from 'react-router-dom';
import cookieStore from '../behavior/cookie/store';
import { useTranslation } from 'react-i18next';
import { Container, Button, Typography, Alert, Stack } from '@mui/material';
import * as Yup from 'yup';
import Form from './form/Form';
import TextField from './form/TextField';

type LoginFormValues = {
  email: string;
  password: string;
};

const LoginPage = () => {
  const { error, loaded, user } = useAppSelector(state => state);
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { t } = useTranslation();

  const initialValues: LoginFormValues = { email: '', password: '' };

  const validationSchema = Yup.object({
    email: Yup.string()
      .email(t('Validation.email', { field: t('LoginPage.email') }))
      .required(t('Validation.required', { field: t('LoginPage.email') })),
    password: Yup.string()
      .min(6, t('Validation.minLength', { field: t('LoginPage.password'), min: 6 }))
      .required(t('Validation.required', { field: t('LoginPage.password') })),
  });

  const handleSubmit = (values: LoginFormValues) => {
    if (!cookieStore.declinedCookieConsent()) {
      dispatch(getLoginAction(values));
    }
  };

  useEffect(() => {
    // loaded && user && navigate('/');
  }, [loaded, user, navigate]);

  return (
    <Container maxWidth="sm">
      <Typography variant="h4" component="h1" gutterBottom>
        {t('LoginPage.login')}
      </Typography>
      <Form
        initialValues={initialValues}
        validationSchema={validationSchema}
        onSubmit={handleSubmit}
      >{() =>
        <Stack spacing={2}>
          <TextField name="email" label={t('LoginPage.email')} type="email" placeholder={t('LoginPage.enterEmail')} />
          <TextField name="password" label={t('LoginPage.password')} type="password" placeholder={t('LoginPage.enterPassword')} />

          {error && <Alert severity="error">{error}</Alert>}

          <Button variant="contained" color="primary" type="submit">
            {t('LoginPage.loginButton')}
          </Button>
        </Stack>}
      </Form>
    </Container>
  );
};

export default LoginPage;
