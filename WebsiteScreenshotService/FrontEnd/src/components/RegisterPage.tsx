import { Container, Button, Form } from "react-bootstrap";
import React, { useEffect, useState } from 'react';
import { useAppSelector } from "../behavior/rootReducer";
import { getRegisterAction } from "../behavior/epic";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import cookieStore from '../behavior/cookie/store';
import { useTranslation } from 'react-i18next';

export default () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [surname, setSurname] = useState('');
    const [name, setName] = useState('');
    const { error, loaded, user } = useAppSelector(state => state);
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { t } = useTranslation();

    useEffect(() => {
        loaded && user && navigate('/');
    }, [loaded, user, navigate]);

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        if (!cookieStore.declinedCookieConsent())
            dispatch(getRegisterAction({ email, password, surname, name }));
    };

    return (
        <Container className="mt-4">
            <h2>{t('RegisterPage.register')}</h2>
            <Form onSubmit={handleSubmit}>
                <Form.Group className="mb-3">
                    <Form.Label>{t('RegisterPage.email')}</Form.Label>
                    <Form.Control
                        type="email"
                        placeholder={t('RegisterPage.enterEmail')}
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                </Form.Group>
                <Form.Group className="mb-3">
                    <Form.Label>{t('RegisterPage.password')}</Form.Label>
                    <Form.Control
                        type="password"
                        placeholder={t('RegisterPage.enterPassword')}
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                </Form.Group>
                <Form.Group className="mb-3">
                    <Form.Label>{t('RegisterPage.surname')}</Form.Label>
                    <Form.Control
                        type="text"
                        placeholder={t('RegisterPage.enterSurname')}
                        value={surname}
                        onChange={(e) => setSurname(e.target.value)}
                    />
                </Form.Group>
                <Form.Group className="mb-3">
                    <Form.Label>{t('RegisterPage.name')}</Form.Label>
                    <Form.Control
                        type="text"
                        placeholder={t('RegisterPage.enterName')}
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                    />
                </Form.Group>
                {error && <Form.Text className="text-danger">{error}</Form.Text>}
                <Button variant="success" type="submit">{t('RegisterPage.registerButton')}</Button>
            </Form>
        </Container>
    );
};
