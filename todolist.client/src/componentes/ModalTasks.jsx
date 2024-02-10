import React, { useState, useEffect } from 'react';
import { Modal, ModalHeader, ModalBody, ModalFooter, Button, FormGroup, Label, Input } from 'reactstrap';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';

const ModalTasks = ({ isOpen, toggleModal, onModalClose, updatePage }) => {
    const [categories, setCategories] = useState([]);
    const [selectedCategory, setSelectedCategory] = useState('');
    const [taskName, setTaskName] = useState('');
    const [deadline, setDeadline] = useState(new Date());
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const response = await fetch('https://localhost:7059/api/Tasks/ListaCategorias');

                if (!response.ok) {
                    throw new Error(`Error: ${response.status}`);
                }

                const data = await response.json();
                setCategories(data);
                setSelectedCategory('');
            } catch (error) {
                console.error(`Error fetching categories: ${error.message}`);
            }
        };

        fetchCategories();
    }, [isOpen]);

    const handleCategoryChange = (e) => {
        setSelectedCategory(e.target.value);
    };

    const handleTaskNameChange = (e) => {
        setTaskName(e.target.value);
    };

    const handleDeadlineChange = (date) => {
        setDeadline(new Date(date.setMinutes(date.getMinutes() - date.getTimezoneOffset())));
    };

    const handleSave = async () => {
        if (!selectedCategory) {
            return setError("La tarea debe tener al menos una categoría asignada");
        }

        const newTask = {
            Name: taskName,
            Deadline: deadline.toISOString(),
            CategoryId: selectedCategory
        };

        try {
            const response = await fetch("https://localhost:7059/api/Tasks/Crear", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(newTask)
            });

            if (!response.ok) {
                throw new Error("Error al crear la tarea");
            }

            setTaskName("");
            setDeadline(null);
            setSelectedCategory(null);
            setError(null);

            handleClose();
           
            updatePage();
            window.location.reload();
        } catch (ex) {
            setError(`Error al guardar la tarea: ${ex.message}`);
        }
    };

    const handleClose = () => {
        toggleModal();
        onModalClose();
    };

    return (
        <Modal isOpen={isOpen} toggle={toggleModal}>
            <ModalHeader toggle={toggleModal}>New Task</ModalHeader>
            <ModalBody>
                <FormGroup>
                    <Label for="taskName" className="fw-bold">Task Name:</Label>
                    <Input type="text" id="taskName" onChange={handleTaskNameChange} />
                </FormGroup>
                <FormGroup>
                    <Label for="categorySelect" className="fw-bold">Category:</Label>
                    <Input
                        type="select"
                        name="category"
                        id="categorySelect"
                        onChange={handleCategoryChange}
                        value={selectedCategory || ''}
                    >
                        <option value="" disabled>Select a category</option>
                        {categories.map((category) => (
                            <option key={category.id} value={category.id}>
                                {category.name}
                            </option>
                        ))}
                    </Input>
                </FormGroup>
                <FormGroup>
                    <Label for="deadline" className="fw-bold">Deadline:</Label>
                    <br />
                    <DatePicker
                        id="deadline"
                        selected={deadline}
                        onChange={handleDeadlineChange}
                        showTimeSelect
                        timeFormat="HH:mm"
                        timeIntervals={15}
                        dateFormat="MMMM d, yyyy h:mm aa"
                        className="form-control"
                    />
                </FormGroup>
            </ModalBody>
            <ModalFooter>
                <Button color="primary" onClick={handleSave}>
                    Save
                </Button>{' '}
                <Button color="secondary" onClick={handleClose}>
                    Close
                </Button>
            </ModalFooter>
        </Modal>
    );
};

export default ModalTasks;
