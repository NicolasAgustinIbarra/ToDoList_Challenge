import React, { useEffect, useState } from 'react';
import moment from 'moment';
import { Card, CardBody, CardTitle, CardText, Button, FormGroup, Label, Input } from 'reactstrap';

const NoteCard = ({ task, onToggleComplete, onDelete, updatePage }) => {
    const [completed, setCompleted] = useState(task.completed);

    useEffect(() => {
        setCompleted(task.completed);
    }, [task.completed]);

    const handleToggleComplete = async () => {
        try {
            const response = await fetch(`https://localhost:7059/api/Tasks/Completar/${task.taskId}`, {
                method: "PUT"
            });

            if (!response.ok) {
                throw new Error(`Error al completar la tarea: ${response.statusText}`);
            }

            // Llama a onToggleComplete para actualizar el estado de la tarea en TableTasks
            onToggleComplete(task.taskId, completed);
        } catch (ex) {
            console.error(`Error al completar la tarea: ${ex.message}`);
        }
    };

    const handleDelete = async () => {
        try {
            const response = await fetch(`https://localhost:7059/api/Tasks/Eliminar/${task.taskId}`, {
                method: "DELETE"
            });

            if (!response.ok) {
                throw new Error("Error al eliminar la tarea");
            }

            onDelete(task.taskId);
        } catch (ex) {
            console.error(`Error al eliminar la tarea: ${ex.message}`);
        }
    };

    // No renderizar la tarjeta si la tarea está completada
    if (completed) {
        return null;
    }

    return (
        <Card className="mb-3" style={{ width: '30%' }}>
            <CardBody>
                <CardTitle tag="h5">{task.taskName}</CardTitle>
                <CardText>
                    <FormGroup check>
                        <Label className="form-label" check>
                            <Input type="checkbox" checked={completed} className="border-2" onChange={handleToggleComplete} />
                            {' '}
                            Completed
                        </Label>
                    </FormGroup>
                    <br />
                    <strong>Deadline:</strong> {moment(task.deadline).format('DD/MM/YYYY')}
                    <br />
                    {/*<strong>Created At:</strong> {moment(task.createdAt).format('DD/MM/YYYY')}*/}
                    {/*<br />*/}
                    {/*<strong>Updated At:</strong> {moment(task.updatedAt).format('DD/MM/YYYY')}*/}
                    {/*<br />*/}
                  {/*  <strong>Categories:</strong> {task.categories.map((category) => category.categoryName).join(', ')}*/}
                </CardText>
            </CardBody>
            <div className="card-footer d-flex justify-content-between">
                {/*<Button color="info" size="sm">*/}
                {/*    Editar*/}
                {/*</Button>*/}
                <Button color="danger" size="sm" onClick={handleDelete}>
                    Delete
                </Button>
            </div>
        </Card>
    );
};

const TableTasks = () => {
    const [tasks, setTasks] = useState([]);

    const fetchTasks = async () => {
        try {
            const response = await fetch("https://localhost:7059/api/Tasks/Lista");
            if (!response.ok) {
                throw new Error(`Error fetching tasks: ${response.statusText}`);
            }
            const data = await response.json();
            setTasks(data);
        } catch (ex) {
            console.error(`Error fetching tasks: ${ex.message}`);
        }
    };

    useEffect(() => {
        fetchTasks();
    }, []);

    const handleToggleComplete = async (taskId, completed) => {
        // Actualizar el estado de las tareas en función de la respuesta de la API
        const updatedTasks = tasks.map(task => {
            if (task.taskId === taskId) {
                return { ...task, completed: !completed };
            }
            return task;
        });

        setTasks(updatedTasks);
    };

    const handleDelete = (taskId) => {
        const updatedTasks = tasks.filter(task => task.taskId !== taskId);
        setTasks(updatedTasks);
    };

    return (
        <div className="d-flex flex-wrap justify-content-around">
            {tasks.map((task) => (
                <NoteCard key={task.taskId} task={task} onToggleComplete={handleToggleComplete} onDelete={handleDelete} />
            ))}
        </div>
    );
};

export default TableTasks;
