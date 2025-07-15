import { Link } from "react-router-dom";
import { Container, Navbar, Nav, Button, Alert, Row, Col } from "react-bootstrap";
import { useState } from "react";
import { useDispatch } from 'react-redux'
import { useAppSelector } from "../behavior/rootReducer";
import { getLogoutAction } from "../behavior/epic";
import Cookiebar from "./Cookiebar";
import cookieStore from '../behavior/cookie/store';
import { BarLoader } from "react-spinners";
import { useTranslation } from 'react-i18next';

const Navigation = () => {
    const dispatch = useDispatch();
    const user = useAppSelector(state => state.user);
    const loaded = useAppSelector(state => state.loaded);
    const cookieCosent = cookieStore.getCookieConsent();
    const [isVisible, setIsVisible] = useState(cookieCosent === null);
    const { t } = useTranslation();

    const handleLogout = () => dispatch(getLogoutAction());

    return (
        <>
            <Navbar bg="dark" variant="dark" expand="lg">
                <Container>
                    <Navbar.Brand as={Link} to="/">{t('Navigation.screenshotService')}</Navbar.Brand>
                    {!isVisible && <Button onClick={() => setIsVisible(true)} variant="outline-secondary" >{t('Navigation.cookiebar')}</Button>}
                    <Nav className="ms-auto">
                        <Nav.Link as={Link} to="/">{t('Navigation.home')}</Nav.Link>
                        <Nav.Link as={Link} to="/privacy-policy" >{t('Navigation.privacyPolicy')}</Nav.Link>
                        {!user && <Nav.Link as={Link} to="/login">{t('Navigation.login')}</Nav.Link>}
                        {!user && <Nav.Link as={Link} to="/register">{t('Navigation.register')}</Nav.Link>}
                        {user && <Nav.Link as={Link} to="/my-account" >{t('Navigation.myAccount')}</Nav.Link>}
                        {user && <Button variant="outline-secondary" onClick={handleLogout}>{t('Navigation.logout')}</Button>}
                    </Nav>
                </Container>
                <Cookiebar setVisibility={setIsVisible} showCookieBar={isVisible} />
            </Navbar>
            <BarLoader width={'100vw'} color="#1370f2" height={5} loading={!loaded} />
            {loaded && cookieCosent === false &&
                <Row className="p-0 m-0 justify-content-center">
                    <Col sm={9}>
                        <Alert variant="danger" className="mt-3">{t('Navigation.cookiesDeclined')}</Alert>
                    </Col>
                </Row>
            }
        </>
    );
};

export default Navigation;