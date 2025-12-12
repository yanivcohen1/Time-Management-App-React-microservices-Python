# React + TypeScript + Vite + Playwright

A modern React application built with TypeScript and Vite, featuring a todo list, user authentication, role-based access control, and nested routing. The app uses Material UI for styling, PrimeReact components, and Axios for API calls.

## Features

- **Todo Management**: Create, edit, and manage todo items
- **User Authentication**: Login/logout with role-based access (admin, user, guest)
- **Nested Routing**: Dynamic routes with React Router v6
- **Theming**: Light/dark mode support
- **Responsive UI**: Material UI and PrimeReact components
- **Data Persistence**: Local storage for user data cache memory
- **Loading Indicators**: Global loading bar for API requests
- **Animations**: Smooth transitions with CSS animations
- **Configurable Backend**: Backend URL loaded from YAML configuration file

## Project Structure

- `src/`
  - `pages/`: Page components with nested routes (Home, About, Login, etc.)
  - `auth/`: Authentication context and logic (AuthContext)
  - `context/`: App-wide state management (AppContext, ToastContext)
  - `components/`: Reusable UI components (CustomButton, CustomSelect)
  - `routes/`: Route protection components (PrivateRoute)
  - `utils/`: Utility functions, storage helpers, and modal components (storage.ts, Modal.tsx)
  - `hooks/`: Custom React hooks (useTheme)
  - `animation/`: CSS animation styles (fade.css, slide-right.css)
  - `assets/`: Static assets
  - `__mocks__/`: Test mocks (axios.js)
- `public/`: Public assets and configuration files (config.dev.yaml, config.prod.yaml, index.html, manifest.json, robots.txt)
- `build/`: Production build output

## Available Scripts

In the project directory, you can run:

### `pnpm dev`

Runs the app in the development mode.\
The server port and backend URL are loaded from `public/config.dev.yaml` (default port: 3001, backend: http://localhost:5000).\
Open [http://localhost:3001](http://localhost:3001) to view it in the browser.

The page will reload if you make edits.\
You will also see any lint errors in the console.

### `pnpm test`

Launches the test runner once.\

### `pnpm test:watch`

Runs Vitest in watch mode for continuous testing during development.\

### `pnpm coverage`

Generates a coverage report using Vitest.\

### `pnpm test:e2e`

Runs end-to-end tests using Playwright.\
you need to run the FE + BE first

### `pnpm build`

Builds the app for production to the `build` folder.\
It correctly bundles React in production mode and optimizes the build for the best performance.

The build is minified and the filenames include the hashes.\
Your app is ready to be deployed!

### `pnpm lint`

Runs ESLint to check for code quality issues and potential errors in the codebase.

### `pnpm preview`

Serves the production build locally for testing and previewing the app before deployment.\
Uses `public/config.prod.yaml` for configuration (backend: https://time-management-api.com).

See the section about [deployment](https://facebook.github.io/create-react-app/docs/deployment) for more information.


## Configuration

The app can be configured using `public/config.dev.yaml` for development and `public/config.prod.yaml` for production:

### Development Configuration (`config.dev.yaml`):

```yaml
port: 3001
backend:
  url: http://localhost:5000
```

### Production Configuration (`config.prod.yaml`):

```yaml
port: 3001
backend:
  url: https://time-management-api.com
```

- `port`: The port on which the React development server runs
- `backend.url`: The base URL for API calls (loaded dynamically at runtime based on NODE_ENV)

## Learn More

You can learn more in the [Create React App documentation](https://facebook.github.io/create-react-app/docs/getting-started).

To learn React, check out the [React documentation](https://reactjs.org/).
