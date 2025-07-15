import { Container, Button, Form } from "react-bootstrap";
import React, { useEffect, useState } from 'react';
import { useAppSelector } from "../behavior/rootReducer";
import { useDispatch } from "react-redux";
import { getLoginAction } from "../behavior/epic";
import { useNavigate } from "react-router-dom";
import cookieStore from '../behavior/cookie/store';
import { useTranslation } from 'react-i18next';

export default () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const { error, loaded, user } = useAppSelector(state => state);
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const { t } = useTranslation();

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (!cookieStore.declinedCookieConsent())
      dispatch(getLoginAction({ email, password }));
  };

  useEffect(() => {
    loaded && user && navigate('/');
  }, [loaded, user, navigate]);

  return (
    <Container className="mt-4">
      <h2>{t('LoginPage.login')}</h2>
      <Form onSubmit={handleSubmit}>
        <Form.Group className="mb-3">
          <Form.Label>{t('LoginPage.email')}</Form.Label>
          <Form.Control
            type="email"
            placeholder={t('LoginPage.enterEmail')}
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />
        </Form.Group>
        <Form.Group className="mb-3">
          <Form.Label>{t('LoginPage.password')}</Form.Label>
          <Form.Control
            type="password"
            placeholder={t('LoginPage.enterPassword')}
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
        </Form.Group>
        {error && <Form.Text className="text-danger">{error}</Form.Text>}
        <Button variant="primary" type="submit">{t('LoginPage.loginButton')}</Button>
      </Form>
    </Container>
  );
};