import { Container, Row, Col, Card } from 'react-bootstrap';
import { useTranslation } from 'react-i18next';

const PrivacyPolicy = () => {
  const { t } = useTranslation();

  return (
    <Container fluid="md" className="my-5">
      <Row>
        <Col>
          <Card className="shadow-lg">
            <Card.Body>
              <Card.Text>
                <div dangerouslySetInnerHTML={{ __html: t('TermsAndConditions.fullPageContent') }} />
              </Card.Text>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

export default PrivacyPolicy;