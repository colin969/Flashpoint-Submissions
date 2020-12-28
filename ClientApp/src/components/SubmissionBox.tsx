import React from 'react';
import { FaDownload, FaEllipsisV, FaThumbsDown, FaThumbsUp, FaTrashAlt } from 'react-icons/fa';
import { deleteSubmission, getSubmissionMeta, updateSubmission } from '../services/SubmissionService';
import { getUser } from '../services/UserService';
import { Meta } from '../types/Meta';
import { Submission } from "../types/Submission";
import { User } from '../types/User';
import { UserPreview } from './UserPreview';

export type SubmissionBoxProps = {
  submission: Submission;
  onSubmissionDeleted?: (sub: Submission) => void;
  onSubmissionApproved?: (sub: Submission) => void;
  onSubmissionDenied?: (sub: Submission) => void;
  userId: string;
  roles: string[];
  autoExpand?: boolean;
}

export function SubmissionBox(props: SubmissionBoxProps) {
  const { submission } = props;
  const [author, setAuthor] = React.useState<User>();
  const [updatedBy, setUpdatedBy] = React.useState<User>();
  const [meta, setMeta] = React.useState<Meta>();
  const [requestedMeta, setRequestedMeta] = React.useState(false);
  const [expanded, setExpanded] = React.useState(false);

  React.useEffect(() => {
    if (props.autoExpand) {
      onToggleExpand();
    }
  }, []);

  React.useEffect(() => {
    getUser(submission.authorId)
    .then((user) => {
      setAuthor(user);
    });
  }, [submission.authorId]);

  React.useEffect(() => {
    if (submission.updatedById) {
      getUser(submission.updatedById)
      .then((user) => {
        setUpdatedBy(user);
      });
    }
  }, [submission.updatedById]);

  const onToggleExpand = React.useCallback(() => {
    setExpanded(!expanded);
    requestMeta();
  }, [expanded]);

  const requestMeta = React.useCallback(() => {
    if (!requestedMeta) {
      setRequestedMeta(true);
      getSubmissionMeta(props.submission.id)
      .then((meta) => setMeta(meta))
      .catch((err) => { /** TODO */ });
    }
  }, [requestedMeta])

  const onDeleteSubmission = () => {
    const { submission } = props;
    if (window.confirm('Really delete this submission?')) {
      deleteSubmission(submission.id)
      .then((newSub) => {
        if (props.onSubmissionDeleted) { props.onSubmissionDeleted(newSub); }
      });
    }
  }

  const onApproveSubmission = () => {
    const updatedSub = {...props.submission}
    updatedSub.status = updatedSub.status === 'Approved' ? 'Awaiting Approval' : 'Approved';
    if (window.confirm(`Really change submission to '${updatedSub.status}'?`)) {
      updateSubmission(updatedSub)
      .then((newSub) => {
        if (props.onSubmissionApproved) { props.onSubmissionApproved(newSub); }
      });
    }
  }

  const onRejectSubmission = () => {
    const updatedSub = {...props.submission}
    updatedSub.status = updatedSub.status === 'Rejected' ? 'Awaiting Approval' : 'Rejected';
    if (window.confirm(`Really change submission to '${updatedSub.status}'?`)) {
      updateSubmission(updatedSub)
      .then((newSub) => {
        if (props.onSubmissionDenied) { props.onSubmissionDenied(newSub); }
      });
    }
  }

  const canDelete = props.submission.status != 'Deleted' && (props.roles.includes('staff') || props.userId == submission.authorId) && props.submission.status !== 'Approved'; 
  const approvalClass = props.submission.status === 'Approved' ? 'approved' :
                      props.submission.status === 'Rejected' ? 'denied' : '';

  return (
    <div className='submission-wrapper'>
      <div className={`submission ${expanded && meta ? 'submission__expanded' : ''}`}>
        { props.submission.status != 'Deleted' && (
          <img 
            className='submission__logo'
            src={props.submission.logoUrl} />
        )}
        <table className='submission__table'>
          <tbody>
            <tr>
              <td>File:</td>
              <td>{submission.fileName}</td>
            </tr>
            <tr>
              <td>Submitted At:</td>
              <td>{submission.submissionDate}</td>
            </tr>
            <tr>
              <td>Status:</td>
              <td>{submission.status}</td>
            </tr>
          </tbody>
        </table>
        <div className='submission__spacer' />
        <table className='submission__users'>
          <tbody>
          { author && (
            <tr className='submission__authored-by'>
              <td>Submitted By:</td>
              <td><UserPreview user={author}/></td>
            </tr>
          )}
          { updatedBy && (
            <tr className='submission__updated-by'>
              <td>Updated By:</td>
              <td><UserPreview user={updatedBy}/></td>
            </tr>
          )}
          </tbody>
        </table>
        <div className='submission__buttons-box'>
          <div className='submission__buttons'>
            {props.submission.status != 'Deleted' && (
              <div className='submission__download'>
                <FaDownload onClick={() => { window.location.href = `/api/submission/${props.submission.id}/download`;}} />
              </div>
            )}
            { canDelete && (
              <div className='submission__delete'>
                <FaTrashAlt onClick={onDeleteSubmission} />
              </div>
            )}
          </div>
          { props.submission.status != 'Deleted' && (
            <div className='submission__buttons'>
                <div className='submission__expand'>
                  <FaEllipsisV onClick={onToggleExpand}/>
                </div>
                <div className={`submission__approve ${approvalClass}`}>
                  <FaThumbsUp onClick={onApproveSubmission}/>
                </div>
                <div className={`submission__deny ${approvalClass}`}>
                  <FaThumbsDown onClick={onRejectSubmission}/>
                </div>
            </div>
          )}
        </div>
      </div>
      { expanded && meta && (
        <div className='submission-details'>
          <table className='submission__table'>
            <tbody>
              {createTableCell('Title: ', meta.title)}
              {createTableCell('Library: ', translateLibrary(meta.library || ''))}
              {createTableCell('Alternate Titles: ', meta.alternateTitles)}
              {createTableCell('Developer: ', meta.developer)}
              {createTableCell('Publisher: ', meta.publisher)}
              {createTableCell('Series: ', meta.series)}
              {createTableCell('Release Date: ', meta.releaseDate)}
              {createTableCell('Source: ', meta.source)}
              {createTableCell('Tags: ', meta.tags)}
              {createTableCell('Application Path: ', meta.applicationPath)}
              {createTableCell('Launch Command: ', meta.launchCommand)}
              {createTableCell('Extreme: ', meta.extreme)}
              {createTableCell('Play Mode: ', meta.playMode)}
              {createTableCell('Status: ', meta.status)}              {createTableCell('Version: ', meta.version)}
              {createTableCell('Notes: ', meta.notes)}
              {createTableCell('Curation Notes: ', meta.curationNotes)}
            </tbody>
          </table>
        </div>
      )}
    </div>
  )
}

function createTableCell(text1: string, text2?: string, placeholder: string = 'N/A', className?: string) {
  return (
    <tr className={'submission__table-cell ' + className}>
      <td>{text1}</td>
      <td>{text2 || placeholder}</td>
    </tr>
  );
}

function translateLibrary(text: string) {
  switch (text){
    case 'arcade':
      return 'Games';
    case 'theatre':
      return 'Animations';
    default:
      return `Unknown (val: ${text})`
  }
}