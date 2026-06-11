import React, { useEffect } from 'react';
import { useAppSelector } from '../../behavior/rootReducer';
import { useDispatch } from 'react-redux';
import { getLoginAction } from '../../behavior/epic';
import { useNavigate } from 'react-router-dom';
import cookieStore from '../../behavior/cookie/store';
import { useTranslation } from 'react-i18next';
import { Container, Button, Typography, Alert, Stack, Link } from '@mui/material';
import * as Yup from 'yup';
import Form from '../form/Form';
import TextField from '../form/TextField';
import { Link as RouterLink } from "react-router-dom";

type LoginFormValues = {
  nickName: string;
  password: string;
};

const LoginPage = () => {
  const { error, loaded, user } = useAppSelector(state => state.basic);
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { t } = useTranslation();

  const initialValues: LoginFormValues = { nickName: '', password: '' };

  const validationSchema = Yup.object({
    nickName: Yup.string()
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
          <TextField name="nickName" label={"Нікнейм"} placeholder={"Ввідеть нікнейм"} />
          <TextField name="password" label={t('LoginPage.password')} type="password" placeholder={t('LoginPage.enterPassword')} />

          {error && <Alert severity="error">{error}</Alert>}

          <Button variant="contained" color="primary" type="submit">
            {t('LoginPage.loginButton')}
          </Button>
          <Link
            component={RouterLink}
            to="/login?mode=reset"
            underline="hover"
          >
            Забули пароль?
          </Link>
        </Stack>}
      </Form>
    </Container>
  );
};

export default LoginPage;
