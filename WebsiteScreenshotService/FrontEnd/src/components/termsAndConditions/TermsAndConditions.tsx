import { Container, Row, Col, Card, ListGroup } from 'react-bootstrap';

const PrivacyPolicy = () => {
  return (
    <Container fluid="md" className="my-5">
      <Row>
        <Col>
          <Card className="shadow-lg">
            <Card.Body>
              <Card.Title className="text-center mb-4">
                <h2>Privacy Policy</h2>
              </Card.Title>
              <Card.Text>
                <h5>Welcome and Introduction</h5>
                <p>
                  Welcome to the Privacy Policy for our <strong>Website Screenshot Service</strong>.
                  Your privacy is important to us. This policy explains what data we collect, how we use it, and how we protect your privacy.
                </p>
                <p>
                  By using our service, you agree to the collection and use of your information as described here. We comply with the <strong>General Data Protection Regulation (GDPR)</strong>, and we’re committed to protecting your data.
                </p>

                <hr />

                <h5>What Data We Collect</h5>
                <p>We collect the following data to provide and improve our services:</p>
                <ListGroup>
                  <ListGroup.Item><strong>Personal Information:</strong></ListGroup.Item>
                  <ListGroup.Item>Email</ListGroup.Item>
                  <ListGroup.Item>Password</ListGroup.Item>
                  <ListGroup.Item>First and Last Name (during registration)</ListGroup.Item>
                  <ListGroup.Item><strong>Usage Data:</strong></ListGroup.Item>
                  <ListGroup.Item>URLs for screenshots</ListGroup.Item>
                  <ListGroup.Item>Screenshot types and quality settings</ListGroup.Item>
                </ListGroup>

                <hr />

                <h5>How We Use Your Data</h5>
                <p>Your data is used in the following ways to improve your experience:</p>
                <ul>
                  <li><strong>To Provide Our Services:</strong> Your data is used to generate the screenshots you request and ensure the service functions smoothly.</li>
                  <li><strong>To Improve the Service:</strong> We analyze data to enhance our platform's performance and your experience.</li>
                  <li><strong>To Secure Your Account:</strong> Your data helps us protect your account and ensure safe usage.</li>
                </ul>

                <hr />

                <h5>Who We Share Your Data With</h5>
                <p>Your personal data is never sold to third parties. However, we may share your information in specific cases:</p>
                <ul>
                  <li><strong>Legal Compliance:</strong> We may disclose data if required by law.</li>
                  <li><strong>Service Providers:</strong> We may share your data with trusted third parties who help us deliver the service (e.g., headless browsers). These providers are bound by strict privacy contracts.</li>
                </ul>

                <hr />

                <h5>Your Rights</h5>
                <p>Under the GDPR, you have the following rights regarding your personal data:</p>
                <ul>
                  <li><strong>Access:</strong> You can access the data we hold about you at any time. <a href='/my-account'>here</a>.</li>
                  <li><strong>Correction:</strong> If any information is incorrect or outdated, you can update it.</li>
                  <li><strong>Deletion:</strong> You have the right to request deletion of your personal data.</li>
                  <li><strong>Data Portability:</strong> You can request to transfer your data to another service.</li>
                  <li><strong>Objection:</strong> You can object to us processing your data for specific purposes.</li>
                </ul>
                <p>If you want to exercise any of these rights, please reach out to us at <strong>haveveq@gmail.com</strong>.</p>

                <hr />

                <h5>Cookies and Tracking</h5>
                <p>We use cookies to make your experience better. Cookies are small files that store data on your device. You can manage your cookie preferences anytime.</p>
                <p><strong>Types of Cookies:</strong></p>
                <ul>
                  <li><strong>Essential Cookies:</strong> Required for the website to function properly.</li>
                </ul>
                <p>You can control and manage your cookies through the cookie consent bar.</p>

                <hr />

                <h5>Policy Changes</h5>
                <p>We may update this policy periodically. The latest version will always be posted here, along with the date it was last updated.</p>

                <hr />

                <h5>Contact Us</h5>
                <p>If you have any questions or concerns about this Privacy Policy, please contact us at:</p>
                <p>📧 <strong>haveveq@gmail.com</strong></p>

                <hr />

                <h5>Consent</h5>
                <p>By continuing to use our service, you acknowledge that you have read and understood this Privacy Policy, and you consent to the collection and use of your information as described.</p>
                <p>By using our application, you also confirm the following:</p>
                <ul>
                  <li>you are older than 16</li>
                  <li>you are not in a country under a U.S. government embargo or designated as a "terrorist-supporting" country;</li>
                  <li>you are not on any U.S. government list of prohibited or restricted parties.</li>
                </ul>

                <hr />

                <footer className="text-center mt-4">
                  <p><small>Last updated: March 23, 2025</small></p>
                </footer>
              </Card.Text>
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </Container>
  );
};

export default PrivacyPolicy;
