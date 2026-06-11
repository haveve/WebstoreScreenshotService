import { memo } from 'react';
import {
    Stack
} from '@mui/material';

import TextFieldWrapper from '../form/TextField';
import ArrayFieldWrapper from '../form/ArrayField';
import SwitchField from '../form/SwitchField';

export interface ModalSectionProps {
    modalEnabled: boolean
}

const modelName = "modal"

const ModalSection = ({
    modalEnabled,
}: ModalSectionProps) => {

    return (
        <>
            <SwitchField
                name='modalEnabled'
                label="Увімкнути налаштування модального вікна"
            />

            {modalEnabled && (
                <Stack spacing={2} sx={{ ml: 2 }}>
                    <SwitchField
                        name={`${modelName}.dismissDialogs`}
                        label="Закривати діалоги"
                    />

                    <SwitchField
                        name={`${modelName}.hidePopups`}
                        label="Приховувати спливаючі вікна"
                    />

                    <ArrayFieldWrapper<string>
                        name={`${modelName}.hideSelectors`}
                        label="Селектори для приховування"
                        emptyValue=""
                        maxLength={15}
                    >
                        {({ index, parentName }) => (
                            <TextFieldWrapper
                                slotProps={{ htmlInput: { maxLength: 200 } }}
                                key={index}
                                name={`${parentName}[${index}]`}
                                label={`Селектор ${index + 1}`}
                                fullWidth
                            />
                        )}
                    </ArrayFieldWrapper>
                </Stack>
            )}
        </>
    );
};

export default memo(ModalSection);