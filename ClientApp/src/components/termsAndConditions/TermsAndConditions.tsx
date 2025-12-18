import { useTranslation } from 'react-i18next';
import {
  Container,
  Card,
  CardContent,
  Typography,
  Box
} from '@mui/material';
import SafeHtml from './SafeHtml';

const PrivacyPolicy = () => {
  const { t } = useTranslation();

  return (
    <Container maxWidth="md">
      <Card sx={{ boxShadow: 6 }}>
        <CardContent>
          <Box>
            <Typography component="div">
              <SafeHtml html={t('TermsAndConditions.fullPageContent')} />
            </Typography>
          </Box>
        </CardContent>
      </Card>
    </Container>
  );
};

export default PrivacyPolicy;
