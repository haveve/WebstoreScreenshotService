import { useState } from "react";
import {
    Button,
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
    Autocomplete
} from "@mui/material";
import EditIcon from "@mui/icons-material/Edit";
import { useDispatch } from "react-redux";

import { Screenshot } from "../../behavior/types";
import {
    updateScreenshotAction,
    getScreenshotsAction
} from "../../behavior/epic";
import { useAppSelector } from "../../behavior/rootReducer";

type Props = {
    screenshot: Screenshot;
};

export const ScreenshotUpdateButton = ({
    screenshot,
}: Props) => {
    const dispatch = useDispatch();
    const allCategories = useAppSelector(store => store.basic.categories?.items) ?? [];

    const [open, setOpen] = useState(false);

    const [title, setTitle] = useState(screenshot.title ?? "");
    const [description, setDescription] = useState(
        screenshot.description ?? ""
    );

    const [selectedCategories, setSelectedCategories] = useState(
        screenshot.categories.filter(c =>
            screenshot.categories?.some(sc => sc.id === c.id)
        )
    );

    const handleClose = () => {
        setOpen(false);
    };

    const handleSave = () => {
        dispatch(
            updateScreenshotAction({
                id: screenshot.id,
                title,
                description,
                categories: selectedCategories.map(x => x.id)
            })
        );

        dispatch(getScreenshotsAction());

        setOpen(false);
    };

    return (
        <>
            <Button
                size="small"
                startIcon={<EditIcon />}
                onClick={() => setOpen(true)}
            >
                Змінити
            </Button>

            <Dialog
                open={open}
                onClose={handleClose}
                maxWidth="sm"
                fullWidth
            >
                <DialogTitle>Змінити інформацію знімка</DialogTitle>

                <DialogContent>
                    <TextField
                        fullWidth
                        margin="normal"
                        label="Заголовок"
                        value={title}
                        onChange={e => setTitle(e.target.value)}
                    />

                    <TextField
                        fullWidth
                        margin="normal"
                        multiline
                        minRows={3}
                        label="Опис"
                        value={description}
                        onChange={e => setDescription(e.target.value)}
                    />

                    <Autocomplete
                        multiple
                        options={allCategories}
                        getOptionLabel={o => o.name}
                        value={selectedCategories}
                        onChange={(_, value) =>
                            setSelectedCategories(value)
                        }
                        renderInput={params => (
                            <TextField
                                {...params}
                                label="Категорії"
                                margin="normal"
                            />
                        )}
                    />
                </DialogContent>

                <DialogActions>
                    <Button onClick={handleClose}>
                        Відмінити
                    </Button>

                    <Button
                        variant="contained"
                        onClick={handleSave}
                    >
                        Зберегти
                    </Button>
                </DialogActions>
            </Dialog>
        </>
    );
};