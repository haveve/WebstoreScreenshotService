import { BrowserRouter as Router, Route, Routes, Link, useNavigate } from "react-router-dom";
import { Container, Navbar, Nav, Button, Form, Row, Col } from "react-bootstrap";
import React, { useState } from 'react';
import axios from 'axios';
import 'bootstrap/dist/css/bootstrap.min.css';

enum ScreenshotType {
  Png = 'Png',
  Jpeg = 'Jpeg'
}

const MainPage = () => {
  const [url, setUrl] = useState('');
  const [screenshotType, setScreenshotType] = useState<ScreenshotType>(ScreenshotType.Png);
  const [quality, setQuality] = useState<number>(0);
  const [fullScreen, setFullScreen] = useState<boolean | null>(null);
  const [clip, setClip] = useState<{ x: number; y: number; width: number; height: number }>({ x: 0, y: 0, width: 0, height: 0 });
  const [error, setError] = useState('');
  const [file, setFile] = useState<Blob | null>(null);

  const validateUrl = (url: string) => {
    const urlPattern = new RegExp('^(https?:\\/\\/)?' + // protocol
      '((([a-z\\d]([a-z\\d-]*[a-z\\d])*)\\.?)+[a-z]{2,}|' + // domain name
      '((\\d{1,3}\\.){3}\\d{1,3}))' + // OR ip (v4) address
      '(\\:\\d+)?(\\/[-a-z\\d%_.~+]*)*' + // port and path
      '(\\?[;&a-z\\d%_.~+=-]*)?' + // query string
      '(\\#[-a-z\\d_]*)?$', 'i'); // fragment locator
    return !!urlPattern.test(url);
  };

  const handleDownload = () => {
    if (file) {
      const url = URL.createObjectURL(file);
      const link = document.createElement('a');
      link.href = url;
      link.download = `screenshot.${screenshotType.toLowerCase()}`;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      URL.revokeObjectURL(url);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validateUrl(url)) {
      setError('Please enter a valid URL.');
      return;
    }
    setError('');
    const data = {
      url,
      screenshotType,
      quality: screenshotType === ScreenshotType.Jpeg ? quality : null,
      fullScreen: clip ? null : fullScreen,
      clip: fullScreen ? null : clip
    };
    try {
      const response = await axios.post(`${process.env.BACK_URL}/MakeScreenshot`, data, { responseType: 'blob', withCredentials: true });
      setFile(response.data);
    } catch (err) {
      setError('Failed to capture screenshot.');
    }
  };

  return (
    <Container className="mt-4">
      <h2 className="text-center">Screenshot Service</h2>
      <p className="text-center">Capture and manage screenshots easily.</p>
      <Form onSubmit={handleSubmit} className="screenshot-form">
        <Form.Group controlId="formUrl" className="mb-3">
          <Form.Label>Enter URL</Form.Label>
          <Form.Control
            type="text"
            placeholder="Enter URL"
            value={url}
            onChange={(e) => setUrl(e.target.value)}
          />
        </Form.Group>
        <Form.Group controlId="formScreenshotType" className="mb-3">
          <Form.Label>Screenshot Type</Form.Label>
          <Form.Control
            as="select"
            value={screenshotType}
            onChange={(e) => setScreenshotType(e.target.value as ScreenshotType)}
          >
            <option value={ScreenshotType.Png}>PNG</option>
            <option value={ScreenshotType.Jpeg}>JPEG</option>
          </Form.Control>
        </Form.Group>
        {screenshotType === ScreenshotType.Jpeg && (
          <Form.Group controlId="formQuality" className="mb-3">
            <Form.Label>Quality (0-100)</Form.Label>
            <Form.Control
              type="range"
              min="0"
              max="100"
              value={quality || 0}
              onChange={(e) => setQuality(Number(e.target.value))}
              placeholder="Enter quality (0-100)"
            />
            <Form.Text className="text-muted">Current Quality: {quality}</Form.Text>
          </Form.Group>
        )}
        <Form.Group controlId="formFullScreen" className="mb-3">
          <Form.Check
            type="checkbox"
            label="Full Screen"
            checked={fullScreen!!}
            onChange={(e) => setFullScreen(e.target.checked)}
          />
        </Form.Group>
        {!fullScreen && (
          <Form.Group controlId="formClip" className="mb-3">
            <Row>
              <Col>
                <Form.Label>X</Form.Label>
                <Form.Control
                  type="number"
                  placeholder="X coordinate"
                  value={clip?.x || ''}
                  onChange={(e) => setClip({ ...clip, x: Number(e.target.value) })}
                />
              </Col>
              <Col>
                <Form.Label>Y</Form.Label>
                <Form.Control
                  type="number"
                  placeholder="Y coordinate"
                  value={clip?.y || ''}
                  onChange={(e) => setClip({ ...clip, y: Number(e.target.value) })}
                />
              </Col>
            </Row>
            <Row className="mt-2">
              <Col>
                <Form.Label>Width</Form.Label>
                <Form.Control
                  type="number"
                  placeholder="Width in pixels"
                  value={clip?.width || ''}
                  onChange={(e) => setClip({ ...clip, width: Number(e.target.value) })}
                />
              </Col>
              <Col>
                <Form.Label>Height</Form.Label>
                <Form.Control
                  type="number"
                  placeholder="Height in pixels"
                  value={clip?.height || ''}
                  onChange={(e) => setClip({ ...clip, height: Number(e.target.value) })}
                />
              </Col>
            </Row>
          </Form.Group>
        )}
        {error && <Form.Text className="text-danger">{error}</Form.Text>}
        <Button variant="primary" type="submit" className="mt-3">Get Screenshot</Button>
        {file && (
          <Button variant="success" onClick={handleDownload} className="mt-3 ms-3">Download Screenshot</Button>
        )}
      </Form>
      {file && (
        <div className="mt-4 text-center">
          <h3>Screenshot:</h3>
          <img src={URL.createObjectURL(file)} alt="Screenshot" className="screenshot-image" />
        </div>
      )}
    </Container>
  );
};


const RegisterPage = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [surname, setSurname] = useState('');
  const [name, setName] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const response = await axios.post(`${process.env.BACK_URL}/Identity/Register`, { email, password, surname, name }, { withCredentials: true });
      console.log('Registration successful:', response.data);
      // Redirect or update UI after successful registration
    } catch (err) {
      if (axios.isAxiosError(err)) {
        setError(err.response?.data || 'Registration failed');
      } else {
        setError('An unexpected error occurred');
      }
    }
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

const LoginPage = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      const response = await axios.post(`${process.env.BACK_URL}Identity/Login`, { email, password }, { withCredentials: true });
      console.log('Login successful:', response.data);
    } catch (err) {
      if (axios.isAxiosError(err)) {
        setError(err.response?.data || 'Login failed');
      } else {
        setError('An unexpected error occurred');
      }
    }
  };

  return (
    <Container className="mt-4">
      <h2>Login</h2>
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
        {error && <Form.Text className="text-danger">{error}</Form.Text>}
        <Button variant="primary" type="submit">Login</Button>
      </Form>
    </Container>
  );
};

const handleLogout = async (navigate: ReturnType<typeof useNavigate>) => {
  try {
    await axios.get(`${process.env.BACK_URL}/Identity/Logout`, { withCredentials: true });
    console.log('Logout successful');
    navigate('/login'); // Redirect to login page after logout
  } catch (err) {
    console.error('Logout failed:', err);
  }
};

const Navigation = () => {
  const navigate = useNavigate();

  return (
    <Navbar bg="dark" variant="dark" expand="lg">
      <Container>
        <Navbar.Brand as={Link} to="/">Screenshot Service</Navbar.Brand>
        <Nav className="ms-auto">
          <Nav.Link as={Link} to="/">Home</Nav.Link>
          <Nav.Link as={Link} to="/login">Login</Nav.Link>
          <Nav.Link as={Link} to="/register">Register</Nav.Link>
          <Button variant="outline-light" onClick={() => handleLogout(navigate)}>Logout</Button>
        </Nav>
      </Container>
    </Navbar>
  );
};

const App = () => {
  return (
    <Router>
      <Navigation />
      <Routes>
        <Route path="/" element={<MainPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
      </Routes>
    </Router>
  );
};

export default App;
