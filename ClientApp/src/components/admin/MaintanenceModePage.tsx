import {
  Box,
  Paper,
  Typography,
  Stack,
  Divider,
} from "@mui/material";
import BuildCircleIcon from "@mui/icons-material/BuildCircle";

export default function MaintenancePage() {
  return (
    <Box
      sx={{
        minHeight: "100vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        background: "linear-gradient(180deg, #fafafa 0%, #f5f5f5 100%)",
      }}
    >
      <Paper
        elevation={3}
        sx={{
          maxWidth: 560,
          width: "100%",
          p: 5,
          borderRadius: 3,
          textAlign: "center",
        }}
      >
        {/* ICON + TITLE */}
        <Stack alignItems="center" spacing={1}>
          <BuildCircleIcon sx={{ fontSize: 56, color: "#ed6c02" }} />

          <Typography variant="h5" fontWeight={700}>
            Ми скоро повернемося
          </Typography>

          <Typography variant="body2" color="text.secondary">
            Наш сервіс тимчасово недоступний, поки ми виконуємо планове
            технічне обслуговування для покращення надійності та продуктивності.
          </Typography>
        </Stack>

        <Divider sx={{ my: 3 }} />

        {/* PROGRESS */}
        <Box mt={3}>
          <Typography
            variant="caption"
            color="text.secondary"
            mt={1}
            display="block"
          >
            Ваші дані в безпеці. Ми працюємо над якнайшвидшим відновленням сервісу.
          </Typography>
        </Box>

        {/* FOOTER */}
        <Typography
          variant="caption"
          color="text.secondary"
          mt={4}
          display="block"
        >
          Screenshot Service
        </Typography>
      </Paper>
    </Box>
  );
}