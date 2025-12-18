import { Container, Stack, Typography, Button, Alert, Link } from '@mui/material';
import { useTranslation } from 'react-i18next';
import * as Yup from 'yup';
import { useAppSelector } from "../behavior/rootReducer";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import cookieStore from '../behavior/cookie/store';
import { getRegisterAction } from "../behavior/epic";
import TextField from './form/TextField';
import Form from './form/Form';
import { useEffect } from 'react';
import CheckboxField from './form/CheckboxField';

interface RegisterValues {
    email: string;
    password: string;
    surname: string;
    name: string;
    agreedWithTerms: boolean
}

const RegisterPage = () => {
    const { error, loaded, user } = useAppSelector(state => state);
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const { t } = useTranslation();

    useEffect(() => {
        // loaded && user && navigate('/');
    }, [loaded, user, navigate]);

    const initialValues: RegisterValues = {
        email: '',
        password: '',
        surname: '',
        name: '',
        agreedWithTerms: true
    };

    const fieldNames = {
        email: t('RegisterPage.email'),
        password: t('RegisterPage.password'),
        surname: t('RegisterPage.surname'),
        name: t('RegisterPage.name'),
        agreedWithTerms: t('RegisterPage.agreedWithTerms'),
    };

    const validationSchema = Yup.object({
        email: Yup.string()
            .email(t('Validation.email', { field: fieldNames.email }))
            .required(t('Validation.required', { field: fieldNames.email })),
        password: Yup.string()
            .min(6, t('Validation.minLength', { field: fieldNames.password, min: 6 }))
            .required(t('Validation.required', { field: fieldNames.password })),
        surname: Yup.string()
            .required(t('Validation.required', { field: fieldNames.surname })),
        name: Yup.string()
            .required(t('Validation.required', { field: fieldNames.name })),
        agreedWithTerms: Yup.boolean()
            .isTrue(t('Validation.agreeTerms'))
    });

    const handleSubmit = (values: RegisterValues) => {
        debugger;
        if (!cookieStore.declinedCookieConsent()) {
            dispatch(getRegisterAction(values));
        }
    };

    return (
        <Container maxWidth="sm">
            <Typography variant="h4" gutterBottom>{t('RegisterPage.register')}</Typography>
            <Form
                initialValues={initialValues}
                validationSchema={validationSchema}
                onSubmit={handleSubmit}
            >{() =>
                <Stack spacing={2}>
                    <TextField
                        name="email"
                        label={fieldNames.email}
                        placeholder={t('RegisterPage.enterEmail')}
                        type="email"
                    />
                    <TextField
                        name="password"
                        label={fieldNames.password}
                        placeholder={t('RegisterPage.enterPassword')}
                        type="password"
                    />
                    <TextField
                        name="surname"
                        label={fieldNames.surname}
                        placeholder={t('RegisterPage.enterSurname')}
                    />
                    <TextField
                        name="name"
                        label={fieldNames.name}
                        placeholder={t('RegisterPage.enterName')}
                    />
                    <CheckboxField
                        name="agreedWithTerms"
                        label={
                            <Link href="/privacy-policy" color="secondary" underline="hover">
                                {fieldNames.agreedWithTerms}
                            </Link>
                        }
                    />
                    {error && <Alert severity="error">{error}</Alert>}
                    <Button type="submit" variant="contained" color="primary">
                        {t('RegisterPage.registerButton')}
                    </Button>
                </Stack>}
            </Form>
        </Container>
    );
};

export default RegisterPage;
