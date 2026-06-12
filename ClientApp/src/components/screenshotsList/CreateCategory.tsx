import { useState } from "react";
import {
    Button,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import { useDispatch } from "react-redux";

import { createCategoryAction } from "../../behavior/epic";

const DEFAULT_COLOR = "#2196f3";

export const CategoryCreateButton = () => {
    const dispatch = useDispatch();

    const [open, setOpen] = useState(false);
    const [name, setName] = useState("");
    const [color, setColor] = useState(DEFAULT_COLOR);

    const handleClose = () => {
        setOpen(false);
        setName("");
        setColor(DEFAULT_COLOR);
    };

    const handleCreate = async () => {
        debugger;
        if (!name.trim()) {
            return;
        }

        dispatch(
            createCategoryAction({
                name,
                color
            })
        );
        handleClose();
    };

    return (
        <>
            <Button
                variant="contained"
                startIcon={<AddIcon />}
                onClick={() => setOpen(true)}
            >
                Створити категарію
            </Button>

            <Dialog
                open={open}
                onClose={handleClose}
                maxWidth="xs"
                fullWidth
            >
                <DialogTitle>Створити категарії</DialogTitle>

                <DialogContent>
                    <TextField
                        label="Назва"
                        fullWidth
                        margin="normal"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                    />

                    <TextField
                        label="Колір"
                        fullWidth
                        type="color"
                        margin="normal"
                        value={color}
                        onChange={(e) => setColor(e.target.value)}
                        placeholder="#2196f3"
                    />
                </DialogContent>

                <DialogActions>
                    <Button onClick={handleClose}>
                        Відмітини
                    </Button>

                    <Button
                        variant="contained"
                        onClick={handleCreate}
                    >
                        Створити
                    </Button>
                </DialogActions>
            </Dialog>
        </>
    );
};