import { Container, Grid, Typography, Button, Box, Paper, AccordionSummary, AccordionDetails, Accordion, Stack, ToggleButtonGroup, ToggleButton, Divider, Chip } from "@mui/material";
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';
import RocketIcon from '@mui/icons-material/Rocket';
import CheckCircleIcon from '@mui/icons-material/CheckCircle';
import DevicesIcon from '@mui/icons-material/Devices';
import ApiIcon from '@mui/icons-material/Api';
import SecurityIcon from '@mui/icons-material/Security';
import { Link as RouterLink } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { useThemeMode } from "../ThemeSettings";
import { format } from "../../utils/string";
import { Duration, SubscriptionType } from "../../behavior/types";
import { useEffect, useState } from "react";
import PointsPicker from "./PointsPicker";
import { SubscriptionCheckoutModal } from "./SubscriptionCheckoutModal";

const HomePage = () => {
  const { t } = useTranslation();
  const { mode } = useThemeMode();

  const [openSubscriptionBasket, setOpenSubscriptionBasket] = useState<SubscriptionType>();
  const [duration, setDuration] = useState(Duration.Monthly);

  useEffect(() => {
    const hash = window.location.hash;
    if (!hash) return;

    const el = document.querySelector(hash);
    el?.scrollIntoView({ behavior: "smooth" });
  }, []);

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
      priceMonth: t("Home.Subscriptions.free.priceMonth"),
      priceYear: t("Home.Subscriptions.free.priceYear"),
      desc: t("Home.Subscriptions.free.desc"),
      benefits: t("Home.Subscriptions.free.benefits").split('\n'),
      subscription: SubscriptionType.Regular,
    },
    {
      plan: t("Home.Subscriptions.premium.plan"),
      priceMonth: t("Home.Subscriptions.premium.priceMonth"),
      priceYear: t("Home.Subscriptions.premium.priceYear"),
      desc: t("Home.Subscriptions.premium.desc"),
      benefits: t("Home.Subscriptions.premium.benefits").split('\n'),
      subscription: SubscriptionType.Pro,
    },
    {
      plan: t("Home.Subscriptions.advanced.plan"),
      priceMonth: t("Home.Subscriptions.advanced.priceMonth"),
      priceYear: t("Home.Subscriptions.advanced.priceYear"),
      desc: t("Home.Subscriptions.advanced.desc"),
      benefits: t("Home.Subscriptions.advanced.benefits").split('\n'),
      subscription: SubscriptionType.Advanced,
    }
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

      <Typography id="subscriptions" variant="h4" gutterBottom sx={{ mt: 6 }}>
        {t("Home.Subscriptions.title")}
      </Typography>

      <Box
        sx={{
          display: "flex",
          justifyContent: "center",
          mt: 4,
          mb: 5
        }}
      >
        <ToggleButtonGroup
          exclusive
          value={duration}
          onChange={(_, value) => {
            if (value !== null)
              setDuration(value);
          }}
        >
          <ToggleButton value={Duration.Monthly}>
            Місячна
          </ToggleButton>

          <ToggleButton
            value={Duration.Yearly}
            sx={{
              borderColor: "#ff9800",
              backgroundColor: "rgba(255, 152, 0, 0.08)",
              fontWeight: 700,
              transform: "scale(1.02)",
              "&:hover": {
                backgroundColor: "rgba(255, 152, 0, 0.14)",
                transform: "scale(1.03)",
              },
            }}
          >
            Річна (Збережи до 17%)
          </ToggleButton>
        </ToggleButtonGroup>
      </Box>

      <Grid container spacing={4}>
        {subscriptions.map((sub, i) => {
          const price = getPlanPrice(sub.subscription, duration);
          const isFree = sub.subscription === SubscriptionType.Regular;
          const isPopular = sub.subscription === SubscriptionType.Pro;
          const priceFormat = duration === Duration.Monthly ? sub.priceMonth : sub.priceYear;

          return <Grid size={{ xs: 12, md: 4 }} key={i}>
            <Paper
              elevation={isPopular ? 8 : 3}
              sx={{
                p: 4,
                height: "100%",
                position: "relative",
                border: isPopular
                  ? "2px solid"
                  : "1px solid transparent",
                borderColor: isPopular
                  ? "primary.main"
                  : "transparent"
              }}

            >
              {isPopular && (
                <Chip
                  label="Most Popular"
                  color="primary"
                  sx={{
                    position: "absolute",
                    top: 16,
                    right: 16
                  }}
                />
              )}

              <Typography variant="h5" fontWeight={700}>
                {sub.plan}
              </Typography>

              <Box sx={{ mt: 2, mb: 2 }}>
                <Typography
                  variant="h4"
                  fontWeight={700}
                >
                  {format(priceFormat, [price])}
                </Typography>

              </Box>

              <Typography variant="body2" mb={3}>
                {sub.desc}
              </Typography>

              <Divider sx={{ mb: 3 }} />

              <Stack spacing={1.5}>
                {sub.benefits.map((b, idx) => (
                  <Box
                    key={idx}
                    sx={{
                      display: "flex",
                      alignItems: "center",
                      gap: 1
                    }}
                  >
                    <CheckCircleIcon
                      color="primary"
                      fontSize="small"
                    />

                    <Typography variant="body2">
                      {b}
                    </Typography>
                  </Box>
                ))}
              </Stack>

              {!isFree && <Button
                fullWidth
                variant={isPopular ? "contained" : "outlined"}
                sx={{ mt: 4 }}
                size="large"
                onClick={() => setOpenSubscriptionBasket(sub.subscription)}
              >
                Придбати
              </Button>}

              <SubscriptionCheckoutModal
                plan={sub.subscription}
                price={price}
                duration={duration}
                open={openSubscriptionBasket === sub.subscription}
                onClose={() => setOpenSubscriptionBasket(undefined)}
                isPopular={isPopular}
                benefits={sub.benefits}
              />
            </Paper>
          </Grid>
        })}
      </Grid>

      <PointsPicker />

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

const getPlanPrice = (plan: SubscriptionType, duration: Duration) => {
  switch (plan) {
    case SubscriptionType.Regular:
      return 0;
    case SubscriptionType.Pro:
      return duration === Duration.Monthly ? 7.99 : 79.99;
    case SubscriptionType.Advanced:
      return duration === Duration.Monthly ? 29.99 : 299.99;
  }
}

export default HomePage;
