import {
    Stack
} from '@mui/material';

import TextFieldWrapper from '../form/TextField';
import SelectFieldWrapper from '../form/SelectField';
import ArrayFieldWrapper from '../form/ArrayField';

import {
    HeaderModel, CookieModel, ColorSchemeOption
} from '../../behavior/types';
import SwitchField from '../form/SwitchField';
import { memo } from 'react';

export interface ModalSectionProps {
    advancedEnabled: boolean
}

const AdvancedSection = ({
    advancedEnabled,
}: ModalSectionProps) => {

    return (
        <>
            <SwitchField
                name='advancedEnabled'
                label="Увімкнути розширену конфігурацію"
            />

            {advancedEnabled && (
                <>
                    <Stack spacing={2} sx={{ ml: 2 }}>
                        <TextFieldWrapper
                            name="locale"
                            label="Локаль (en-US)"
                            fullWidth
                        />

                        <TextFieldWrapper
                            name="timezoneId"
                            label="Часовий пояс (Europe/Kyiv)"
                            fullWidth
                        />

                        <SelectFieldWrapper
                            name="colorScheme"
                            label="Колірна схема"
                            options={[
                                { value: ColorSchemeOption.Light, label: 'Світла' },
                                { value: ColorSchemeOption.Dark, label: 'Темна' },
                                { value: ColorSchemeOption.NoPreference, label: 'Без налаштувань' }
                            ]}
                        />

                        <TextFieldWrapper
                            slotProps={{ htmlInput: { maxLength: 200 } }}
                            name="waitForSelector"
                            label="Очікувати селектор"
                            fullWidth
                        />

                        <SelectFieldWrapper
                            name="blockResources"
                            label="Блокувати ресурси"
                            multiple
                            options={[
                                { value: 'Images', label: 'Зображення' },
                                { value: 'Fonts', label: 'Шрифти' },
                                { value: 'Media', label: 'Медіа' },
                                { value: 'Scripts', label: 'Скрипти' },
                                { value: 'Stylesheets', label: 'Таблиці стилів' }
                            ]}
                        />

                        {/* Headers */}
                        <ArrayFieldWrapper<HeaderModel>
                            name="headers"
                            label="Заголовки"
                            emptyValue={{ name: '', value: '' }}
                            maxLength={20}
                        >
                            {({ index, parentName }) => (
                                <Stack
                                    direction="row"
                                    key={index}
                                    spacing={1}
                                    sx={{ width: '100%' }}
                                >
                                    <TextFieldWrapper
                                        slotProps={{ htmlInput: { maxLength: 100 } }}
                                        name={`${parentName}[${index}].name`}
                                        label="Назва заголовка"
                                        fullWidth
                                    />

                                    <TextFieldWrapper
                                        slotProps={{ htmlInput: { maxLength: 1000 } }}
                                        name={`${parentName}[${index}].value`}
                                        label="Значення заголовка"
                                        fullWidth
                                    />
                                </Stack>
                            )}
                        </ArrayFieldWrapper>

                        {/* Cookies */}
                        <ArrayFieldWrapper<CookieModel>
                            name="cookies"
                            label="Cookies"
                            emptyValue={{
                                name: '',
                                value: '',
                                domain: '',
                                path: '/',
                                secure: false,
                                httpOnly: false,
                                expires: new Date().toISOString().slice(0, 16)
                            }}
                            maxLength={15}
                        >
                            {({ index, parentName }) => (
                                <Stack
                                    key={index}
                                    spacing={1}
                                    sx={{
                                        width: '100%',
                                        p: 2,
                                        border: '1px solid #ddd',
                                        borderRadius: 1
                                    }}
                                >
                                    <Stack direction="row" spacing={1}>
                                        <TextFieldWrapper
                                            slotProps={{ htmlInput: { maxLength: 100 } }}
                                            name={`${parentName}[${index}].name`}
                                            label="Назва Cookie"
                                            fullWidth
                                        />

                                        <TextFieldWrapper
                                            slotProps={{ htmlInput: { maxLength: 1000 } }}
                                            name={`${parentName}[${index}].value`}
                                            label="Значення Cookie"
                                            fullWidth
                                        />
                                    </Stack>

                                    <Stack direction="row" spacing={1}>
                                        <TextFieldWrapper
                                            slotProps={{ htmlInput: { maxLength: 200 } }}
                                            name={`${parentName}[${index}].domain`}
                                            label="Домен"
                                            placeholder=".example.com"
                                            fullWidth
                                        />

                                        <TextFieldWrapper
                                            slotProps={{ htmlInput: { maxLength: 100 } }}
                                            name={`${parentName}[${index}].path`}
                                            label="Шлях"
                                            placeholder="/"
                                            fullWidth
                                        />
                                    </Stack>

                                    <TextFieldWrapper
                                        type="datetime-local"
                                        label="Термін дії (UTC)"
                                        name={`${parentName}[${index}].expires`}
                                        inputProps={{
                                            min: new Date().toISOString().slice(0, 16),
                                            max: new Date(new Date().setMonth(new Date().getMonth() + 6))
                                                .toISOString()
                                                .slice(0, 16)
                                        }}
                                        fullWidth
                                    />

                                    <SelectFieldWrapper
                                        fullWidth
                                        name={`${parentName}[${index}].sameSite`}
                                        label="Same site"
                                        options={[
                                            { value: 'Strict', label: 'Strict' },
                                            { value: 'Lax', label: 'Lax' },
                                            { value: 'None', label: 'None' }
                                        ]}
                                    />

                                    <Stack direction="row" spacing={2}>
                                        <SwitchField
                                            name={`${parentName}[${index}].secure`}
                                            label="Secure"
                                        />

                                        <SwitchField
                                            name={`${parentName}[${index}].httpOnly`}
                                            label="HttpOnly"
                                        />
                                    </Stack>
                                </Stack>
                            )}
                        </ArrayFieldWrapper>
                    </Stack>
                </>
            )}
        </>
    );
};

export default memo(AdvancedSection);