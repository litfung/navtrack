import React, { FormEvent, useState, useContext } from "react";
import { Link, useHistory } from "react-router-dom";
import { LoginModel, DefaultLoginModel } from "./LoginModel";
import { Errors, ApiError } from "services/HttpClient/HttpClient";
import AppContext from "services/AppContext";
import InputError, { HasErrors, AddError } from "components/Common/InputError";
import { AccountApi } from "services/Api/AccountApi";
import LoginLayout from "components/Framework/Layouts/Login/LoginLayout";
import Icon from "components/Framework/Util/Icon";

export default function Login() {
  const [login, setLogin] = useState<LoginModel>(DefaultLoginModel);
  const [showLoadingIndicator, setShowLoadingIndicator] = useState(false);
  const [errors, setErrors] = useState<Errors>({});
  const history = useHistory();
  const { appContext, setAppContext } = useContext(AppContext);

  const signIn = async (e: FormEvent) => {
    e.preventDefault();

    const errors = validateModel(login);

    if (HasErrors(errors)) {
      setErrors(errors);
    } else {
      setShowLoadingIndicator(true);

      AccountApi.login(login.email, login.password)
        .then(() => {
          setAppContext({ ...appContext, authenticated: true })

          history.push("/");
        })
        .catch((error: ApiError) => {
          setErrors(error.errors);
          setShowLoadingIndicator(false);
        });
    }
  };

  return (
    <LoginLayout>
      <div className="max-w-xs w-full flex flex-col items-center">
        <div className="h-16 m-3 ">
          <a href="https://www.navtrack.io">
            <img src="/navtrack.png" width="64" className="mb-4" alt="Navtrack" />
          </a>
        </div>
        <div className="shadow-xl bg-white rounded px-8 w-full bg-gray-100">
          <div className="text-center my-6">Sign in to Navtrack</div>
          <form onSubmit={(e) => signIn(e)}>
            <div className="mb-4">
              <input className="shadow appearance-none rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none" id="email" type="email" placeholder="Email"
                value={login.email} onChange={(e) => setLogin({ ...login, email: e.target.value })}
              />
              <InputError name="email" errors={errors} />
            </div>
            <div className="mb-4">
              <input className="shadow appearance-none rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:border focus:border-gray-900" id="password" type="password" placeholder="Password"
                value={login.password} onChange={(e) => setLogin({ ...login, password: e.target.value })}
              />
              <InputError name="password" errors={errors} />
            </div>
            <div className="flex justify-center my-6">
              <button className="shadow-md bg-gray-800 hover:bg-gray-700 text-white font-bold py-2 px-4 rounded focus:outline-none" type="submit">
                <Icon className="fa-spinner fa-spin" show={showLoadingIndicator} /> Sign in
            </button>
            </div>
          </form>
        </div>
        <div className="h-20 flex w-full">
          <div className="flex-grow">
            <Link to="/register" className="text-white text-xs">Create new account</Link>
          </div>
          <div className="flex-grow text-right">
            {/* <Link to="/forgotpassword" className="text-white text-xs">Forgot password?</Link> */}
          </div>
        </div>
      </div>
    </LoginLayout>
  );
};

const validateModel = (login: LoginModel): Record<string, string[]> => {
  const errors: Record<string, string[]> = {};

  if (login.email.length === 0) {
    AddError<LoginModel>(errors, "email", "Email is required.");
  }
  if (login.password.length === 0) {
    AddError<LoginModel>(errors, "password", "Password is required.");
  }

  return errors;
};