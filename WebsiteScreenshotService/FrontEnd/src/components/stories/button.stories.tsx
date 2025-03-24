import { Button } from 'react-bootstrap';
import { Meta } from '@storybook/react';
import { fn } from '@storybook/test';
import 'bootstrap/dist/css/bootstrap.min.css';

const meta: Meta<typeof Button> = {
    title: 'Button',
    component: Button,
    args: {
        onClick: fn()
    },
    argTypes: {
        variant: {
            type: 'string',
            description: 'Bootstrap button variant',
            defaultValue: 'primary',
            options: ['primary', 'secondary', 'success', 'danger', 'warning', 'info', 'light', 'dark', 'link'],
            control: {
                type: 'select',
            },
        },
        size: {
            type: 'string',
            description: 'Size of the button',
            defaultValue: 'md',
            options: ['sm', 'md', 'lg'],
            control: { type: 'select' },
        },
        type: {
            type: 'string',
            description: 'Button type',
            defaultValue: 'button',
            options: ['button', 'submit', 'reset'],
            control: { type: 'select' },
        },
        children: {
            type: 'string',
            name: 'label',
            description: 'Button label',
            defaultValue: 'Click me',
            control: { type: 'text' },
        },
    },
}

export default meta;

export const Default = {
    args: {
        variant: 'primary',
        size: 'md',
        type: 'button',
        children: 'Click me',
    }
};

export const Secondary = {
    args: {
        variant: 'secondary',
        size: 'md',
        type: 'button',
        children: 'Click me',
    }
};

export const OutlinedSecondary = {
    args: {
        variant: 'outline-secondary',
        size: 'md',
        type: 'button',
        children: 'Click me',
    }
};