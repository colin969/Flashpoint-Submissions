import React, { Component } from 'react';
import { Route, Switch } from 'react-router';
import { Dashboard } from './components/pages/Dashboard';
import { Home } from './components/pages/Home';
import { Layout } from './components/Layout';
import './custom.css';
import './theme.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import { AuthorizedRoute } from './components/AuthorizedRoute';
import { User } from './types/User';
import { ReviewHub } from './components/pages/ReviewHub';
import { UsersPage } from './components/pages/UsersPage';
import { Page404 } from './components/pages/Page404';
import { SubmissionPage } from './components/pages/SubmissionPage';

export type AppProps = {}

export type AppState = {
  user?: User;
  loaded: boolean;
}

export default class App extends Component<AppProps, AppState> {
  static displayName = 'Flashpoint Submissions';

  constructor(props: AppProps) {
    super(props);
    this.state = {
      user: undefined,
      loaded: false
    }
  }

  async componentDidMount() {
    try{
      const res = await fetch('api/user/');
      if (res.status === 200) {
        const data = await res.json();
        const user = ParseUser(data);
        this.setState({ user: user });
      }
    } catch (e) {
    }
    this.setState({ loaded: true });
  }

  render () {
    return (
      <Layout user={this.state.user}>
        <Switch>
          <Route exact path='/' component={Home} />
          <AuthorizedRoute path='/dashboard' render={() => (
            <Dashboard user={this.state.user}/>  
          )} 
          authorized={!!this.state.user}
          loaded={this.state.loaded} />
          <AuthorizedRoute path='/submission/:subId' render={(props) => (
            <SubmissionPage user={this.state.user} {...props} />
          )}
          authorized={!!this.state.user}
          loaded={this.state.loaded} />
          <AuthorizedRoute path='/review' render={() => (
            <ReviewHub user={this.state.user}/>
          )} 
          authorized={!!this.state.user && this.state.user.roles.includes('staff')}
          loaded={this.state.loaded} />
          <AuthorizedRoute path='/users' render={() => (
            <UsersPage />
          )} 
          authorized={!!this.state.user && this.state.user.roles.includes('staff')}
          loaded={this.state.loaded} />
          <Route component={Page404} />
        </Switch>
      </Layout>
    );
  }
}

function ParseUser(data: any): User {
  return {
    id: data['id'],
    username: data['username'],
    email: data['email'],
    imageUrl: data['imageUrl'] || '',
    roles: data['roles']
  }
}
