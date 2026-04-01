import { Container, Grid, Typography, Button, Box, Paper, AccordionSummary, AccordionDetails, Accordion, Stack } from "@mui/material";
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import RocketIcon from '@mui/icons-material/Rocket';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import DevicesIcon from '@mui/icons-material/Devices';
import ApiIcon from '@mui/icons-material/Api';
import SecurityIcon from '@mui/icons-material/Security';
import { Link as RouterLink } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useThemeMode } from "./ThemeSettings";
import { format } from "../utils/string";

const HomePage = () => {
  const { t } = useTranslation();
  const { mode } = useThemeMode();

  const features = [
    { title: t("Home.Features.fast.title"), desc: t("Home.Features.fast.desc"), icon: <RocketIcon sx={{ fontSize: 60, color: '#1976d2' }} /> },
    { title: t("Home.Features.quality.title"), desc: t("Home.Features.quality.desc"), icon: <CheckCircleIcon sx={{ fontSize: 60, color: '#1976d2' }} /> },
    { title: t("Home.Features.flexibility.title"), desc: t("Home.Features.flexibility.desc"), icon: <DevicesIcon sx={{ fontSize: 60, color: '#1976d2' }} /> },
    { title: t("Home.Features.api.title"), desc: t("Home.Features.api.desc"), icon: <ApiIcon sx={{ fontSize: 60, color: '#1976d2' }} /> },
    { title: t("Home.Features.security.title"), desc: t("Home.Features.security.desc"), icon: <SecurityIcon sx={{ fontSize: 60, color: '#1976d2' }} /> },
  ];

  const faqs = [
    { q: t("Home.FAQs.0.q"), a: t("Home.FAQs.0.a") },
    { q: t("Home.FAQs.1.q"), a: t("Home.FAQs.1.a") },
    { q: t("Home.FAQs.2.q"), a: t("Home.FAQs.2.a") },
    { q: t("Home.FAQs.3.q"), a: t("Home.FAQs.3.a") },
    { q: t("Home.FAQs.4.q"), a: t("Home.FAQs.4.a") },
  ];

  const subscriptions = [
    {
      plan: t("Home.Subscriptions.free.plan"),
      price: t("Home.Subscriptions.free.price"),
      desc: t("Home.Subscriptions.free.desc"),
      benefits: t("Home.Subscriptions.free.benefits").split('\n'),
      isFree: true
    },
    {
      plan: t("Home.Subscriptions.premium.plan"),
      price: t("Home.Subscriptions.premium.price"),
      desc: t("Home.Subscriptions.premium.desc"),
      benefits: t("Home.Subscriptions.premium.benefits").split('\n'),
      isFree: false
    },
  ];

  return (
    <Container maxWidth="lg">
      {/* Hero Section */}
      <Grid container spacing={4} alignItems="center">
        <Grid size={{ xs: 12, md: 6 }}>
          <Typography variant="h2" component="h1" gutterBottom>
            {t("Home.Hero.title")}
          </Typography>
          <Typography variant="body1" gutterBottom>
            {t("Home.Hero.subtitle")}
          </Typography>
          <Button component={RouterLink} to="/make-screenshot" variant="contained" color="primary" size="large">
            {t("Home.Hero.cta")}
          </Button>
        </Grid>
        <Grid size={{ xs: 12, md: 6 }}>
          <Box
            component="img"
            src={`/images/hero-${mode}.jpg`}
            alt={t("Home.Hero.imgAlt") as string}
            loading="lazy"
            sx={{ width: "100%", borderRadius: 2 }}
          />
        </Grid>
      </Grid>

      {/* Features Section */}
      <Grid
        container
        spacing={4}
        sx={{ display: 'flex', justifyContent: 'center', alignItems: 'top', mt: 8 }}
      >
        {features.map((feature, index) => (
          <Grid
            size={{ xs: 12, md: 4 }}
            key={index}
            sx={{ display: 'flex', justifyContent: 'center' }}
          >
            <Paper
              elevation={3}
              sx={{
                p: 3,
                textAlign: 'center',
                display: 'flex',
                flexDirection: 'column',
                alignItems: 'center',
                justifyContent: 'flex-start',
                height: '100%',
                width: '100%'
              }}
            >
              <Box sx={{ mb: 2 }}>{feature.icon}</Box>
              <Typography variant="h6" gutterBottom>
                {feature.title}
              </Typography>
              <Typography variant="body2">{feature.desc}</Typography>
            </Paper>
          </Grid>
        ))}
      </Grid>

      {/* CTA Section */}
      <Grid container spacing={4} sx={{ mt: 8, mb: 6 }} justifyContent="center">
        <Grid size={{ xs: 12, md: 8 }} textAlign="center">
          <Typography variant="h4" gutterBottom>
            {t("Home.CTA.title")}
          </Typography>
          <Button component={RouterLink} to="/make-screenshot" variant="contained" color="primary" size="large">
            {t("Home.CTA.button")}
          </Button>
        </Grid>
      </Grid>

      <Typography variant="h4" gutterBottom sx={{ mt: 6 }}>
        {t("Home.Subscriptions.title")}
      </Typography>
      <Grid container spacing={4}>
        {subscriptions.map((sub, i) => (
          <Grid size={{ xs: 12, md: 6 }} key={i}>
            <Paper
              elevation={3}
              sx={{
                p: 3,
                height: '100%'
              }}
            >
              <Typography variant="h5">{sub.plan}{format(sub.price, sub.isFree ? 0 : 9.99)}</Typography>
              <Typography variant="body2" mb={2}>{sub.desc}</Typography>
              <ul>
                {sub.benefits.map((b, idx) => (
                  <li key={idx}><Typography variant="body2">{b}</Typography></li>
                ))}
              </ul>
            </Paper>
          </Grid>
        ))}
      </Grid>

      {/* FAQ */}
      <Typography variant="h4" gutterBottom sx={{ mt: 6 }}>
        {t("Home.FAQs.title")}
      </Typography>
      {faqs.map((faq, index) => (
        <Accordion key={index} sx={{ mt: 2 }}>
          <AccordionSummary expandIcon={<ExpandMoreIcon />}>
            <Typography variant="subtitle1">{faq.q}</Typography>
          </AccordionSummary>
          <AccordionDetails>
            <Stack spacing={1}>
              <Typography variant="body2">{faq.a}</Typography>
            </Stack>
          </AccordionDetails>
        </Accordion>
      ))}

    </Container>
  );
};

export default HomePage;
