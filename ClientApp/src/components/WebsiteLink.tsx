import { Box, Typography, Link } from "@mui/material";
import { useState } from "react";
import { Dialog, DialogTitle, DialogContent, DialogActions, Button } from "@mui/material";

const WebsiteLink = ({ url }: { url: string }) => {
    const [open, setOpen] = useState(false);

    const handleConfirm = () => {
        window.open(url, "_blank", "noopener,noreferrer");
        setOpen(false);
    };

    return (
        <>
            <Box mt={2}>
                <Link
                    component="button"
                    variant="caption"
                    onClick={(e) => {
                        e.stopPropagation();
                        setOpen(true)
                    }}
                    underline="hover"
                >
                    {url}
                </Link>
            </Box>

            <Dialog open={open} onClose={() => setOpen(false)}>
                <DialogTitle>Leave this site?</DialogTitle>

                <DialogContent>
                    <Typography variant="body2">
                        You are about to open an external website:
                        <br />
                        <b>{url}</b>
                    </Typography>
                </DialogContent>

                <DialogActions>
                    <Button onClick={() => setOpen(false)}>
                        Cancel
                    </Button>

                    <Button
                        onClick={handleConfirm}
                        variant="contained"
                        color="warning"
                    >
                        Continue
                    </Button>
                </DialogActions>
            </Dialog>
        </>
    );
};

export default WebsiteLink;