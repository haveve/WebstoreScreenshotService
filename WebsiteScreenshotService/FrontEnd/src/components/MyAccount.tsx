import React, { useEffect } from "react";
import { useAppSelector } from "../behavior/rootReducer";
import { useDispatch } from "react-redux";
import { getReceiveUserAction } from "../behavior/epic";
import { Container, Row, Col, Card } from "react-bootstrap";
import { SubscriptionType } from "../behavior/types";
import { useTranslation } from 'react-i18next';

const MyAccount: React.FC = () => {
    const user = useAppSelector((state) => state.user!);
    const loaded = useAppSelector((state) => state.loaded);
    const dispatch = useDispatch();
    const { t } = useTranslation();

    useEffect(() => {
        dispatch(getReceiveUserAction());
    }, [dispatch]);

    if (!loaded) return null;

    return (
        <Container>
            <Row className="justify-content-md-center">
                <Col md={8}>
                    <Card>
                        <Card.Header as="h2">{t('MyAccount.myAccount')}</Card.Header>
                        <Card.Body>
                            <Row>
                                <Col>
                                    <Card.Text>
                                        <strong>{t('MyAccount.email')}</strong> {user.email}
                                    </Card.Text>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <Card.Text>
                                        <strong>{t('MyAccount.firstName')}</strong> {user.name}
                                    </Card.Text>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <Card.Text>
                                        <strong>{t('MyAccount.lastName')}</strong> {user.surname}
                                    </Card.Text>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <Card.Text>
                                        <strong>{t('MyAccount.screenshotsLeft')}</strong> {user.subscriptionPlan.screenshotLeft}
                                        <br />
                                        <strong>{t('MyAccount.plan')}</strong> {getPlanDescription(user.subscriptionPlan.type, t)}
                                    </Card.Text>
                                </Col>
                            </Row>
                            <Row>
                                <Col>
                                    <Card.Text>
                                        <br />
                                        {t('MyAccount.gdprContact')} <b>{t('MyAccount.gdpr')}</b>{t('MyAccount.contactUs')} {" "}
                                        <a href="/privacy-policy">
                                            {t('MyAccount.contactInformation')}
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


function getPlanDescription(plan: SubscriptionType, t: any) {
    switch (plan) {
        case SubscriptionType.Regular:
            return t('MyAccount.regularPlan');
        default:
            return t('MyAccount.unknownPlan');
    }
}

export default MyAccount;