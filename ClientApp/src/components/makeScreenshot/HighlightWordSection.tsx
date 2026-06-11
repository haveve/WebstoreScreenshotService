import React, { memo } from 'react';
import {
    Stack
} from '@mui/material';

import TextFieldWrapper from '../form/TextField';
import SwitchField from '../form/SwitchField';
import ColorPicker from '../form/ColorPicker';

export interface ModalSectionProps {
    highlightEnabled: boolean
}

const highlightWordName = "highlightWord"

const HighlightWordSection = ({
    highlightEnabled,
}: ModalSectionProps) => {

    return (
        <>
            <SwitchField
                name='highlightEnabled'
                label="Увімкнути підсвічування слова"
            />

            {highlightEnabled && (
                <Stack spacing={2} sx={{ ml: 2 }}>
                    <TextFieldWrapper
                        slotProps={{ htmlInput: { maxLength: 200 } }}
                        name={`${highlightWordName}.word`}
                        label="Слово для підсвічування"
                        fullWidth
                    />
                    <ColorPicker
                        name={`${highlightWordName}.color`}
                        label="Колір підсвічування"
                        placeholder="#FFFF00"
                        fullWidth
                    />
                </Stack>
            )}
        </>
    );
};

export default memo(HighlightWordSection);