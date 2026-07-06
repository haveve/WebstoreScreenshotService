import { Box, Typography, IconButton, Paper, Tooltip } from "@mui/material";
import ContentCopyIcon from "@mui/icons-material/ContentCopy";
import { ScreenshotOptionsModel } from "../../behavior/types";
import { memo, useRef, useState } from "react";

export interface JsonPreviewProps {
  values: ScreenshotOptionsModel;
}

const JsonPreview = ({ values }: JsonPreviewProps) => {
  const json = JSON.stringify(values, null, 2);
  const [open, setOpen] = useState(false);
  const timeoutRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const handleCopy = async () => {
    await navigator.clipboard.writeText(json);

    setOpen(true);

    if (timeoutRef.current)
      clearTimeout(timeoutRef.current);

    timeoutRef.current = setTimeout(() => setOpen(false), 1000);
  };

  return (
    <Paper
      elevation={3}
      sx={{
        p: 2,
        height: "100%",
        position: "sticky",
        top: 20,
        backgroundColor: "#0f172a",
        color: "#e2e8f0",
        overflow: "auto"
      }}
    >
      <Box display="flex" justifyContent="space-between" alignItems="center">
        <Typography variant="subtitle1">Запит у форматі JSON</Typography>
        <Tooltip
          title={"Copied!"}
          open={open}
          disableFocusListener
          disableHoverListener
          disableTouchListener
        >
          <IconButton
            onClick={handleCopy}
            size="small"
            sx={{ color: "#e2e8f0" }}
          >
            <ContentCopyIcon fontSize="small" />
          </IconButton>
        </Tooltip>
      </Box>

      <Box
        component="pre"
        sx={{
          mt: 2,
          fontSize: "12px",
          whiteSpace: "pre-wrap",
          wordBreak: "break-word"
        }}
      >
        {json}
      </Box>
    </Paper>
  );
};

export default memo(JsonPreview);