import React, { useEffect } from "react";
import { useAppSelector } from "../behavior/rootReducer";
import { useDispatch } from "react-redux";
import { getReceiveUserAction } from "../behavior/epic";
import { Container, Row, Col, Card } from "react-bootstrap";
import { SubscriptionPlan, SubscriptionType } from "../behavior/types";

const MyAccount: React.FC = () => {
    const user = useAppSelector((state) => state.user!);
    const loaded = useAppSelector((state) => state.loaded);
    const dispatch = useDispatch();

    useEffect(() => {
        dispatch(getReceiveUserAction());
    }, [dispatch]);

    if (!loaded) return null;

    return (
        <Container>
            <Row className="justify-content-md-center">
                <Col md={8}>
                    <Card>
                        <Card.Header as="h2">My Account</Card.Header>
                        <Card.Body>
                            <Row>
                                <Col>
                                    <Card.Text>
                                        <strong>Email:</strong> {user.email}
                                    </Card.Text>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <Card.Text>
                                        <strong>First Name:</strong> {user.name}
                                    </Card.Text>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <Card.Text>
                                        <strong>Last Name:</strong> {user.surname}
                                    </Card.Text>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <Card.Text>
                                        <strong>Screenshots Left:</strong> {user.subscriptionPlan.screenshotLeft}
                                        <br />
                                        <strong>Plan:</strong> {getPlanDescription(user.subscriptionPlan.type)}
                                    </Card.Text>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <Card.Text>
                                        <br />
                                        To manage your data according to <b>GDPR</b>, contact us -{" "}
                                        <a href="/privacy-policy">
                                            You can find contact information here
                                        </a>
                                    </Card.Text>
                                </Col>
                            </Row>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
};


function getPlanDescription(plan: SubscriptionType) {
    switch (plan) {
        case SubscriptionType.Regular:
            return "Regular plan";
        default:
            return "Unknown plan";
    }
}

export default MyAccount;