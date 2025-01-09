import PropTypes from 'prop-types';
import React, { Component } from 'react';
import keyboardShortcuts, { shortcuts } from 'Components/keyboardShortcuts';
import IconButton from 'Components/Link/IconButton';
import Link from 'Components/Link/Link';
import { icons } from 'Helpers/Props';
import translate from 'Utilities/String/translate';
import ArtistSearchInputConnector from './ArtistSearchInputConnector';
import KeyboardShortcutsModal from './KeyboardShortcutsModal';
import PageHeaderActionsMenu from './PageHeaderActionsMenu';
import styles from './PageHeader.css';

class PageHeader extends Component {

  //
  // Lifecycle

  constructor(props, context) {
    super(props);

    this.state = {
      isKeyboardShortcutsModalOpen: false
    };
  }

  componentDidMount() {
    this.props.bindShortcut(shortcuts.OPEN_KEYBOARD_SHORTCUTS_MODAL.key, this.onOpenKeyboardShortcutsModal);
  }

  //
  // Control

  onOpenKeyboardShortcutsModal = () => {
    this.setState({ isKeyboardShortcutsModalOpen: true });
  };

  //
  // Listeners

  onKeyboardShortcutsModalClose = () => {
    this.setState({ isKeyboardShortcutsModalOpen: false });
  };

  //
  // Render

  render() {
    const {
      onSidebarToggle
    } = this.props;

    return (
      <div className={styles.header}>
        <div className={styles.logoContainer}>
          <Link
            className={styles.logoLink}
            to={'/'}
          >
            <img
              className={styles.logo}
              src={`${window.Lidarr.urlBase}/Content/Images/logo.svg`}
              alt="Lidarr Logo"
            />
          </Link>
        </div>

        <div className={styles.sidebarToggleContainer}>
          <IconButton
            id="sidebar-toggle-button"
            name={icons.NAVBAR_COLLAPSE}
            onPress={onSidebarToggle}
          />
        </div>

        <ArtistSearchInputConnector />

        <div className={styles.right}>

          <IconButton
            className={styles.translation}
            title={translate('SuggestTranslationChange')}
            name={icons.TRANSLATE}
            to="https://translate.servarr.com/projects/servarr/lidarr/"
            size={24}
          />

          <PageHeaderActionsMenu
            onKeyboardShortcutsPress={this.onOpenKeyboardShortcutsModal}
          />
        </div>

        <KeyboardShortcutsModal
          isOpen={this.state.isKeyboardShortcutsModalOpen}
          onModalClose={this.onKeyboardShortcutsModalClose}
        />
      </div>
    );
  }
}

PageHeader.propTypes = {
  onSidebarToggle: PropTypes.func.isRequired,
  bindShortcut: PropTypes.func.isRequired
};

export default keyboardShortcuts(PageHeader);
