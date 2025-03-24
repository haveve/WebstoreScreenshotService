import { Link } from "react-router-dom";
import { Container, Navbar, Nav, Button, Alert, Row, Col } from "react-bootstrap";
import { useState } from "react";
import { useDispatch } from 'react-redux'
import { useAppSelector } from "../behavior/rootReducer";
import { getLogoutAction } from "../behavior/epic";
import Cookiebar from "./Cookiebar";
import cookieStore from '../behavior/cookie/store';
import { BarLoader } from "react-spinners";

export default () => {
    const dispatch = useDispatch();
    const user = useAppSelector(state => state.user);
    const loaded = useAppSelector(state => state.loaded);
    const cookieCosent = cookieStore.getCookieConsent();
    const [isVisible, setIsVisible] = useState(cookieCosent === null);

    const handleLogout = () => dispatch(getLogoutAction());

    return (
        <>
            <Navbar bg="dark" variant="dark" expand="lg">
                <Container>
                    <Navbar.Brand as={Link} to="/">Screenshot Service</Navbar.Brand>
                    {!isVisible && <Button onClick={() => setIsVisible(true)} variant="outline-secondary" >Cookiebar</Button>}
                    <Nav className="ms-auto">
                        <Nav.Link as={Link} to="/">Home</Nav.Link>
                        <Nav.Link as={Link} to="/privacy-policy" >Privacy & Policy</Nav.Link>
                        {!user && <Nav.Link as={Link} to="/login">Login</Nav.Link>}
                        {!user && <Nav.Link as={Link} to="/register">Register</Nav.Link>}
                        {user && <Nav.Link as={Link} to="/my-account" >MyAccount</Nav.Link>}
                        {user && <Button variant="outline-secondary" onClick={handleLogout}>Logout</Button>}
                    </Nav>
                </Container>
                <Cookiebar setVisibility={setIsVisible} showCookieBar={isVisible} />
            </Navbar>
            <BarLoader width={'100vw'} color="#1370f2" height={5} loading={!loaded} />
            {loaded && cookieCosent === false &&
                <Row className="p-0 m-0 justify-content-center">
                    <Col sm={9}>
                        <Alert variant="danger" className="mt-3">You have declined cookies usage. You won't be able to login and register.</Alert>
                    </Col>
                </Row>
            }
        </>
    );
};