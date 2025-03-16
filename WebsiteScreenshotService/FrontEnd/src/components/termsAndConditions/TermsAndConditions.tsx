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
                <h5>Introduction</h5>
                <p>
                  Welcome to the Privacy Policy of the <strong>Website Screenshot Service</strong>.
                </p>
                <p>This document outlines how we collect, use, and protect your data while ensuring compliance with the <strong>General Data Protection Regulation (GDPR)</strong>.</p>
                
                <hr />
                
                <h5>Data Collection</h5>
                <p>We collect the following types of data:</p>
                <ListGroup>
                  <ListGroup.Item><strong>Personal Information:</strong></ListGroup.Item>
                  <ListGroup.Item>Email</ListGroup.Item>
                  <ListGroup.Item>Password</ListGroup.Item>
                  <ListGroup.Item>First and Last Name (during registration)</ListGroup.Item>
                  <ListGroup.Item><strong>Usage Data:</strong></ListGroup.Item>
                  <ListGroup.Item>URLs for screenshots</ListGroup.Item>
                  <ListGroup.Item>Screenshot types</ListGroup.Item>
                  <ListGroup.Item>Quality settings</ListGroup.Item>
                </ListGroup>

                <hr />

                <h5>Data Usage</h5>
                <p>Your data is used to:</p>
                <ul>
                  <li><strong>Provide and Improve the Service:</strong> Enhance user experience and service quality.</li>
                  <li><strong>Authenticate You Securely:</strong> Ensure your account's safety and verify your identity.</li>
                  <li><strong>Generate and Manage Screenshots Efficiently:</strong> Process and deliver screenshots as requested.</li>
                </ul>

                <hr />

                <h5>Data Sharing</h5>
                <p>We <strong>do not</strong> share your personal data with third parties except in the following cases:</p>
                <ul>
                  <li><strong>Legal Requirements:</strong> When required by law.</li>
                  <li><strong>Service Provision:</strong> To fulfill your request for screenshot services (includes third-party services such as headless browsers).</li>
                </ul>

                <hr />

                <h5>Data Retention</h5>
                <p>We retain your personal data only as long as necessary to provide our services and comply with legal obligations. Data will be deleted upon account closure or upon your request.</p>

                <hr />

                <h5>Data Security</h5>
                <p>We implement robust security measures to protect your data, including encryption and secure storage practices. However, no method of internet transmission or electronic storage is 100% secure.</p>

                <hr />

                <h5>User Rights</h5>
                <p>Under GDPR, you have the following rights:</p>
                <ul>
                  <li><strong>Access:</strong> <a href='/my-account'>Request access to your personal data.</a></li>
                  <li><strong>Rectification:</strong> Correct any inaccuracies in your data.</li>
                  <li><strong>Erasure:</strong> Request deletion of your data.</li>
                  <li><strong>Restriction:</strong> Restrict the processing of your data.</li>
                  <li><strong>Data Portability:</strong> Transfer your data to another service.</li>
                  <li><strong>Objection:</strong> Object to data processing.</li>
                </ul>
                <p>To exercise these rights, please contact us at <strong>haveveq@gmail.com</strong>.</p>

                <hr />

                <h5>Cookies</h5>
                <p>We use cookies to enhance your experience. Cookies are small data files stored on your device.</p>
                <p><strong>Types of cookies used:</strong></p>
                <ul>
                  <li><strong>Essential Cookies:</strong> Necessary for basic functions.</li>
                </ul>
                <p>You can manage your cookie preferences through the cookie consent bar.</p>

                <hr />

                <h5>Children's Privacy</h5>
                <p>Our service is not intended for children under the age of 16. We do not knowingly collect data from children.</p>

                <hr />

                <h5>Changes to This Policy</h5>
                <p>We may update this Privacy Policy periodically. Any changes will be posted on this page with the updated date.</p>

                <hr />

                <h5>Contact Us</h5>
                <p>If you have any questions about this Privacy Policy, please contact us at:</p>
                <p>📧 <strong>haveveq@gmail.com</strong></p>

                <hr />

                <h5>Consent</h5>
                <p>By accessing and using our service, you acknowledge that you have read, understood, and agreed to our Privacy Policy and its terms. Your continued use of the service constitutes your explicit consent to the collection, use, and disclosure of your information as described in the Privacy Policy.</p>

                <hr />

                <footer className="text-center mt-4">
                  <p><small>Last updated: March 16, 2025</small></p>
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
