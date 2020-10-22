import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { User } from '../types/User';
import { LoginLogoutButton } from './LoginLogoutButton';
import './NavMenu.css';

export type NavMenuProps = {
  user?: User
};

export type NavMenuState = {
  collapsed: boolean;
}

export class NavMenu extends Component<NavMenuProps, NavMenuState>{
  static displayName = NavMenu.name;

  constructor (props: NavMenuProps) {
    super(props);

    this.toggleNavbar = this.toggleNavbar.bind(this);
    this.state = {
      collapsed: true
    };
  }

  toggleNavbar () {
    this.setState({
      collapsed: !this.state.collapsed
    });
  }

  render () {
    const loggedIn = !!this.props.user;
    return (
      <header>
        <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
          <Container>
            <NavbarBrand tag={Link} to="/">Flashpoint Submissions</NavbarBrand>
            <NavbarToggler onClick={this.toggleNavbar} className="mr-2" />
            <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!this.state.collapsed} navbar>
              <ul className="navbar-nav flex-grow">
                <NavItem>
                  <NavLink tag={Link} className="text-dark" to="/">Home</NavLink>
                </NavItem>
                { this.props.user && this.props.user.roles.includes('staff') && (
                  <NavItem>
                    <NavLink tag={Link} className='text-dark' to='/users'>Users</NavLink>
                  </NavItem>
                )}
                { this.props.user && this.props.user.roles.includes('staff') && (
                  <NavItem>
                    <NavLink tag={Link} className='text-dark' to='/review'>Review Hub</NavLink>
                  </NavItem>
                )}
                { loggedIn && (
                  <NavItem>
                    <NavLink tag={Link} className='text-dark' to='/dashboard'>Dashboard</NavLink>
                  </NavItem>
                )}
                <LoginLogoutButton loggedIn={loggedIn}/>
              </ul>
            </Collapse>
          </Container>
        </Navbar>
      </header>
    );
  }
}
