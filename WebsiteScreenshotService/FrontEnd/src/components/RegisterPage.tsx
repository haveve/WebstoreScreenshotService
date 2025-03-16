import { Container, Button, Form } from "react-bootstrap";
import React, { useEffect, useState } from 'react';
import { useAppSelector } from "../behavior/rootReducer";
import { getRegisterAction } from "../behavior/epic";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import cookieStore from '../behavior/cookie/store';

export default () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [surname, setSurname] = useState('');
    const [name, setName] = useState('');
    const { error, loaded, user } = useAppSelector(state => state);
    const dispatch = useDispatch();
    const navigate = useNavigate();

    useEffect(() => {
        loaded && user && navigate('/');
    }, [loaded, user]);

    const handleSubmit = (e: React.FormEvent) => {
        e.preventDefault();

        if (!cookieStore.declinedCookieConsent())
            dispatch(getRegisterAction({ email, password, surname, name }));
    };

    return (
        <Container className="mt-4">
            <h2>Register</h2>
            <Form onSubmit={handleSubmit}>
                <Form.Group className="mb-3">
                    <Form.Label>Email</Form.Label>
                    <Form.Control
                        type="email"
                        placeholder="Enter email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                    />
                </Form.Group>
                <Form.Group className="mb-3">
                    <Form.Label>Password</Form.Label>
                    <Form.Control
                        type="password"
                        placeholder="Enter password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                    />
                </Form.Group>
                <Form.Group className="mb-3">
                    <Form.Label>Surname</Form.Label>
                    <Form.Control
                        type="text"
                        placeholder="Enter surname"
                        value={surname}
                        onChange={(e) => setSurname(e.target.value)}
                    />
                </Form.Group>
                <Form.Group className="mb-3">
                    <Form.Label>Name</Form.Label>
                    <Form.Control
                        type="text"
                        placeholder="Enter name"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                    />
                </Form.Group>
                {error && <Form.Text className="text-danger">{error}</Form.Text>}
                <Button variant="success" type="submit">Register</Button>
            </Form>
        </Container>
    );
};
