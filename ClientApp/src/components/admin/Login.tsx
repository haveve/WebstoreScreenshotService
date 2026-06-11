import React, { useState } from "react";
import {
  Box,
  Button,
  Link,
  Paper,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { Link as RouterLink } from "react-router-dom";

export default function AdminLoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  return (
    <Box
      sx={{
        minHeight: "100vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        background:
          "linear-gradient(180deg, #fafafa 0%, #f4f4f4 100%)",
        p: 3,
      }}
    >
      <Paper
        elevation={3}
        sx={{
          width: 420,
          p: 4,
          borderRadius: 3,
        }}
      >
        <Stack spacing={3}>
          {/* HEADER */}
          <Box>
            <Typography variant="h5" fontWeight={700}>
              Вхід адміністратора
            </Typography>

            <Typography variant="body2" color="text.secondary">
              Увійдіть, щоб отримати доступ до адміністративної панелі.
            </Typography>
          </Box>

          {/* EMAIL */}
          <TextField
            label="Нікнейм"
            type="email"
            fullWidth
            value={email}
            onChange={(e) => setEmail(e.target.value)}
          />

          {/* PASSWORD */}
          <TextField
            label="Пароль"
            type="password"
            fullWidth
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />

          {/* LOGIN */}
          <Button
            variant="contained"
            size="large"
            fullWidth
          >
            Продовжити
          </Button>

          <Link
            component={RouterLink}
            to="/login?mode=reset"
            underline="hover"
          >
            Забули пароль?
          </Link>
        </Stack>
      </Paper>
    </Box>
  );
}