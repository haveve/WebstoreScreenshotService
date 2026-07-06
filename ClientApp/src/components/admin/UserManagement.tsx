import React, { useMemo, useState } from "react";
import {
  Box,
  Typography,
  Paper,
  Stack,
  Chip,
  TextField,
  Button,
  Drawer,
  TablePagination,
} from "@mui/material";
import CheckCircleIcon from "@mui/icons-material/CheckCircle";
import WarningAmberIcon from "@mui/icons-material/WarningAmber";
import BlockIcon from "@mui/icons-material/Block";

/* ---------------- TYPES ---------------- */

type SubscriptionType = "Regular" | "Pro" | "Advanced";

interface PaymentAttemptEntity {
  id: string;
  orderId: string;
  userId: string;
  amount: number;
  provider: string;
  createdAt: string;
}

interface User {
  id: string;
  name: string;
  email: string;
  isDisactivated: boolean;
  subscriptionType: SubscriptionType;
  transactions: PaymentAttemptEntity[];
}

/* ---------------- MOCK ---------------- */

const mockUsers: User[] = Array.from({ length: 25 }).map((_, i) => ({
  id: String(i + 1),
  name: `User ${i + 1}`,
  email: `user${i + 1}@mail.com`,
  isDisactivated: i % 5 === 0,
  subscriptionType: i % 3 === 0 ? "Pro" : "Regular",
  transactions:
    i % 2 === 0
      ? [
        {
          id: `t-${i}`,
          orderId: `o-${i}`,
          userId: String(i + 1),
          amount: 20 + i,
          provider: "Stripe",
          createdAt: "2025-10-01",
        },
      ]
      : [],
}));

/* ---------------- HELPERS ---------------- */

const getTxColor = (count: number) => {
  if (count === 0) return "#9e9e9e";
  if (count === 1) return "#1976d2";
  return "#2e7d32";
};

/* ---------------- COMPONENT ---------------- */

export default function UsersAdminPanel() {
  const [users] = useState<User[]>(mockUsers);
  const [search, setSearch] = useState("");

  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(5);

  const [selectedUser, setSelectedUser] = useState<User | null>(null);

  /* ---------------- FILTERS ---------------- */

  const [statusFilter, setStatusFilter] = useState<
    "all" | "active" | "disabled"
  >("all");

  const [refundFilter, setRefundFilter] = useState<
    "all" | 0 | 1 | 2 | 3 | 4 | "5+"
  >("all");

  /* ---------------- FILTER LOGIC ---------------- */

  const filtered = useMemo(() => {
    return users.filter((u) => {
      const matchesSearch = !search || u.name == search;

      if (!matchesSearch) return false;

      // ФІЛЬТР СТАТУСУ
      if (statusFilter === "active" && u.isDisactivated) return false;
      if (statusFilter === "disabled" && !u.isDisactivated) return false;

      // ФІЛЬТР ПОВЕРНЕНЬ
      const txCount = u.transactions.length;

      if (refundFilter !== "all") {
        if (refundFilter === "5+") {
          if (txCount < 5) return false;
        } else {
          if (txCount !== refundFilter) return false;
        }
      }

      return true;
    });
  }, [users, search, statusFilter, refundFilter]);

  /* ---------------- PAGINATION ---------------- */

  const paginated = useMemo(() => {
    return filtered.slice(
      page * rowsPerPage,
      page * rowsPerPage + rowsPerPage
    );
  }, [filtered, page, rowsPerPage]);

  /* ---------------- UI ---------------- */

  return (
    <Box p={3} sx={{ background: "#fafafa", minHeight: "100vh" }}>
      {/* HEADER */}
      <Stack direction="row" justifyContent="space-between" mb={2}>
        <Typography variant="h5" fontWeight={700}>
          Керування користувачами
        </Typography>
      </Stack>

      {/* SEARCH */}
      <TextField
        fullWidth
        placeholder="Пошук за ім’ям..."
        value={search}
        onChange={(e) => {
          setSearch(e.target.value);
          setPage(0);
        }}
        sx={{ mb: 2 }}
      />

      {/* FILTERS */}
      <Stack direction="row" spacing={3} mb={3} flexWrap="wrap">
        {/* STATUS FILTER */}
        <Stack direction="row" spacing={1} alignItems="center">
          <Typography variant="caption">Статус:</Typography>

          <Chip
            label="Всі"
            clickable
            color={statusFilter === "all" ? "primary" : "default"}
            onClick={() => setStatusFilter("all")}
          />
          <Chip
            label="Активні"
            clickable
            color={statusFilter === "active" ? "success" : "default"}
            onClick={() => setStatusFilter("active")}
          />
          <Chip
            label="Вимкнені"
            clickable
            color={statusFilter === "disabled" ? "error" : "default"}
            onClick={() => setStatusFilter("disabled")}
          />
        </Stack>

        {/* REFUND FILTER */}
        <Stack direction="row" spacing={1} alignItems="center">
          <Typography variant="caption">
            Повернення (останні 6 місяців):
          </Typography>

          {["all", 0, 1, 2, 3, 4, "5+"].map((val) => (
            <Chip
              key={String(val)}
              label={val}
              clickable
              color={refundFilter === val ? "primary" : "default"}
              onClick={() => setRefundFilter(val as any)}
            />
          ))}
        </Stack>
      </Stack>

      {/* LIST */}
      <Stack spacing={2}>
        {paginated.map((user) => {
          const txCount = user.transactions.length;

          return (
            <Paper
              key={user.id}
              sx={{
                p: 2,
                borderLeft: `5px solid ${getTxColor(txCount)}`,
                opacity: user.isDisactivated ? 0.6 : 1,
                cursor: "pointer",
              }}
            >
              <Stack direction="row" alignItems="center" spacing={2}>
                <Box
                  sx={{ flexGrow: 1 }}
                  onClick={() => setSelectedUser(user)}
                >
                  <Typography fontWeight={700}>
                    {user.name} ({user.email})
                  </Typography>

                  <Stack direction="row" spacing={1} mt={0.5}>
                    <Chip size="small" label={user.subscriptionType} />

                    {user.isDisactivated ? (
                      <Chip size="small" color="error" label="ВИМКНЕНО" />
                    ) : (
                      <Chip size="small" color="success" label="АКТИВНИЙ" />
                    )}

                    {txCount === 0 && (
                      <Chip
                        size="small"
                        icon={<BlockIcon />}
                        label="0 ПОВЕРНЕНЬ"
                        sx={{ backgroundColor: "#9e9e9e", color: "#fff" }}
                      />
                    )}

                    {txCount === 1 && (
                      <Chip
                        size="small"
                        icon={<WarningAmberIcon />}
                        label="1 ПОВЕРНЕННЯ"
                        sx={{ backgroundColor: "#1976d2", color: "#fff" }}
                      />
                    )}

                    {txCount >= 2 && (
                      <Chip
                        size="small"
                        icon={<CheckCircleIcon />}
                        label="2+ ПОВЕРНЕНЬ"
                        sx={{ backgroundColor: "#2e7d32", color: "#fff" }}
                      />
                    )}
                  </Stack>
                </Box>
              </Stack>
            </Paper>
          );
        })}
      </Stack>

      {/* PAGINATION */}
      <TablePagination
        component="div"
        count={filtered.length}
        page={page}
        onPageChange={(_, p) => setPage(p)}
        rowsPerPage={rowsPerPage}
        onRowsPerPageChange={(e) => {
          setRowsPerPage(parseInt(e.target.value, 10));
          setPage(0);
        }}
      />

      {/* DRAWER */}
      <Drawer
        anchor="right"
        open={!!selectedUser}
        onClose={() => setSelectedUser(null)}
      >
        {selectedUser && (
          <Box sx={{ width: 380, p: 3 }}>
            <Typography variant="h6" fontWeight={700}>
              {selectedUser.name}
            </Typography>

            <Typography color="text.secondary" mb={2}>
              {selectedUser.email}
            </Typography>

            <Chip
              label={selectedUser.isDisactivated ? "ВИМКНЕНО" : "АКТИВНИЙ"}
              color={selectedUser.isDisactivated ? "error" : "success"}
              sx={{ mb: 2 }}
            />

            <Button
              fullWidth
              variant="contained"
              color={selectedUser.isDisactivated ? "success" : "error"}
            >
              {selectedUser.isDisactivated ? "Увімкнути" : "Вимкнути"}
            </Button>

            <Box mt={3}>
              <Typography fontWeight={600} mb={1}>
                Повернуті основні транзакції (останні 6 місяців)
              </Typography>

              {selectedUser.transactions.length === 0 ? (
                <Typography color="text.secondary">
                  Немає транзакцій
                </Typography>
              ) : (
                selectedUser.transactions.map((t) => (
                  <Box
                    key={t.id}
                    sx={{
                      p: 1.5,
                      mb: 1,
                      border: "1px solid #eee",
                      borderRadius: 1,
                      backgroundColor: "#fff",
                    }}
                  >
                    <Stack spacing={0.5}>
                      <Stack direction="row" justifyContent="space-between">
                        <Typography variant="caption" fontWeight={600}>
                          Замовлення
                        </Typography>
                        <Typography variant="caption">
                          {t.orderId}
                        </Typography>
                      </Stack>

                      <Stack direction="row" justifyContent="space-between">
                        <Typography variant="caption" fontWeight={600}>
                          Транзакція
                        </Typography>
                        <Typography variant="caption">{t.id}</Typography>
                      </Stack>

                      <Stack direction="row" justifyContent="space-between">
                        <Typography variant="caption" fontWeight={600}>
                          Провайдер
                        </Typography>
                        <Typography variant="caption">
                          {t.provider}
                        </Typography>
                      </Stack>

                      <Stack direction="row" justifyContent="space-between">
                        <Typography variant="caption" fontWeight={600}>
                          Сума
                        </Typography>
                        <Typography variant="caption">
                          ${t.amount}
                        </Typography>
                      </Stack>

                      <Stack direction="row" justifyContent="space-between">
                        <Typography variant="caption" fontWeight={600}>
                          Створено
                        </Typography>
                        <Typography
                          variant="caption"
                          color="text.secondary"
                        >
                          {t.createdAt}
                        </Typography>
                      </Stack>
                    </Stack>
                  </Box>
                ))
              )}
            </Box>
          </Box>
        )}
      </Drawer>
    </Box>
  );
}