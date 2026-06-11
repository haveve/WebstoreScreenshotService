import React, { useMemo, useState } from "react";
import {
  Alert,
  Box,
  Button,
  Divider,
  Paper,
  Stack,
  TextField,
  Typography,
} from "@mui/material";
import { useTranslation } from "react-i18next";

export default function AdminRegistrationPage() {
  const [nickname, setNickname] = useState("");

  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] =
    useState("");

  const { t } = useTranslation();

  const passwordsMatch = useMemo(() => {
    return (
      password.length > 0 &&
      confirmPassword.length > 0 &&
      password === confirmPassword
    );
  }, [password, confirmPassword]);

  const isValid =
    nickname.length >= 3 &&
    password.length >= 10 &&
    passwordsMatch;

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
          width: 480,
          p: 4,
          borderRadius: 3,
        }}
      >
        <Stack spacing={3}>
          {/* HEADER */}
          <Box>
            <Typography variant="h5" fontWeight={700}>
              Реєстрація адміністратора
            </Typography>

            <Typography variant="body2" color="text.secondary">
              Завершіть початкове налаштування облікового запису адміністратора.
            </Typography>
          </Box>

          {/* SECURITY WARNING */}
          <Alert severity="warning">
            Це одноразове посилання для реєстрації, яке автоматично
            втрачає чинність.
          </Alert>

          <Divider />

          {/* NICKNAME */}
          <TextField
            label="Нікнейм"
            fullWidth
            value={nickname}
            onChange={(e) => setNickname(e.target.value)}
          />

          {/* PASSWORD */}
          <TextField
            label="Пароль"
            type="password"
            fullWidth
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />

          {/* CONFIRM */}
          <TextField
            label="Підтвердження пароля"
            type="password"
            fullWidth
            value={confirmPassword}
            error={
              confirmPassword.length > 0 &&
              !passwordsMatch
            }
            helperText={
              confirmPassword.length > 0 &&
              !passwordsMatch
                ? "Паролі не співпадають"
                : undefined
            }
            onChange={(e) =>
              setConfirmPassword(e.target.value)
            }
          />

          {/* SUBMIT */}
          <Button
            variant="contained"
            size="large"
            fullWidth
            disabled={!isValid}
          >
            Створити обліковий запис адміністратора
          </Button>
        </Stack>
      </Paper>
    </Box>
  );
}