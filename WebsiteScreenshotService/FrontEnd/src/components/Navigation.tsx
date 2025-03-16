import { Link, useNavigate } from "react-router-dom";
import { Container, Navbar, Nav, Button } from "react-bootstrap";
import { useEffect } from "react";
import { useDispatch } from 'react-redux'
import { useAppSelector } from "../behavior/rootReducer";
import { getLogoutAction } from "../behavior/epic";

export default () => {
    const navigate = useNavigate();
    const dispatch = useDispatch();
    const user = useAppSelector(state => state.user);

    useEffect(() => {
        user && navigate('/login');
    }, [user])

    const handleLogout = () => dispatch(getLogoutAction());

    return (
        <Navbar bg="dark" variant="dark" expand="lg">
            <Container>
                <Navbar.Brand as={Link} to="/">Screenshot Service</Navbar.Brand>
                <Nav className="ms-auto">
                    <Nav.Link as={Link} to="/">Home</Nav.Link>
                    {!user && <Nav.Link as={Link} to="/login">Login</Nav.Link>}
                    {!user && <Nav.Link as={Link} to="/register">Register</Nav.Link>}
                    {user && <Button variant="outline-light" onClick={handleLogout}>Logout</Button>}
                </Nav>
            </Container>
        </Navbar>
    );
};